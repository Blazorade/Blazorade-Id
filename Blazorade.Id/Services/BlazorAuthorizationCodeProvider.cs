using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Components.Pages;
using Blazorade.Core.Components;
using Blazorade.Core.Interop;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text.Json;
using Blazorade.Id.Model;
using Blazorade.Id.Configuration;
using Blazored.LocalStorage;
using System.Collections.Specialized;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// An <see cref="IAuthorizationCodeProvider"/> implementation for use in Blazor Server and Blazor WebAssembly applications.
    /// </summary>
    public class BlazorAuthorizationCodeProvider : IAuthorizationCodeProvider
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public BlazorAuthorizationCodeProvider(
            IEndpointService endpointService, 
            ICodeChallengeService codeChallengeService,
            IPropertyStore propertyStore,
            NavigationManager navMan,
            BlazoradeIdScriptService scriptService,
            IRedirectUriProvider redirUriProvider,
            ILocalStorageService localStorage,
            IOptions<AuthorityOptions> authOptions
        ) {
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.CodeChallengeService = codeChallengeService ?? throw new ArgumentNullException(nameof(codeChallengeService));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.ScriptService = scriptService ?? throw new ArgumentNullException(nameof(scriptService));
            this.RedirUriProvider = redirUriProvider ?? throw new ArgumentNullException(nameof(redirUriProvider));
            this.LocalStore = new BrowserLocalStoragePropertyStore(localStorage);
        }

        private readonly IEndpointService EndpointService;
        private readonly ICodeChallengeService CodeChallengeService;
        private readonly IPropertyStore PropertyStore;
        private readonly NavigationManager NavMan;
        private readonly AuthorityOptions AuthOptions;
        private readonly BlazoradeIdScriptService ScriptService;
        private readonly IRedirectUriProvider RedirUriProvider;
        private readonly IPropertyStore LocalStore;

        private const int AuthorizeTimeout = 300000;

        /// <inheritdoc/>
        public async Task<AuthorizationCodeResult> GetAuthorizationCodeAsync(GetTokenOptions options, CancellationToken cancellationToken = default)
        {
            var redirUrl = this.AuthOptions.RedirectUri?.Length > 0
                ? this.AuthOptions.RedirectUri
                : this.RedirUriProvider.GetRedirectUri().ToString();

            var codeVerifier = this.CodeChallengeService.CreateCodeVerifier();
            await this.PropertyStore.SetCodeVerifierAsync(codeVerifier);

            var nonce = this.CreateNonce();
            await this.PropertyStore.SetNonceAsync(nonce);

            var scopes = string.Join(' ', options.Scopes ?? []);
            await this.PropertyStore.SetScopeAsync(scopes);

            var uriBuilder = await this.EndpointService.CreateAuthorizationUriBuilderAsync(this.CodeChallengeService);
            uriBuilder
                .WithClientId(this.AuthOptions.ClientId)
                .WithResponseType(ResponseType.Code)
                .WithResponseMode(ResponseMode.Query)
                .WithRedirectUri(redirUrl)
                .WithScope(scopes)
                .WithNonce(nonce)
                .WithCodeChallenge(codeVerifier);

            if(options.Prompt.HasValue) uriBuilder.WithPrompt(options.Prompt.Value);

            var lastSuccessfulTimestamp = await this.LocalStore.GetLastSuccessfulAuthCodeTimestampAsync();

            AuthorizationCodeResult result = null != lastSuccessfulTimestamp && !options.Prompt.RequiresInteraction()
                ? await this.AttemptIFrameAsync(uriBuilder, options)
                : new AuthorizationCodeResult();

            if (string.IsNullOrEmpty(result.Code) || result.FailureReason != null)
            {
                // IFrame attempt failed, was not attempted, or authentication has never succeeded before, try popup.
                result = await this.AttemptPopupAsync(uriBuilder, options);
            }

            if(result.Code?.Length > 0 && result.FailureReason == null)
            {
                await this.LocalStore.SetLastSuccessfulAuthCodeTimestampAsync(DateTime.UtcNow);
            }

            return result;
        }


        private async Task<AuthorizationCodeResult> AttemptIFrameAsync(EndpointUriBuilder authorizeUriBuilder, GetTokenOptions getOptions)
        {
            authorizeUriBuilder.WithPrompt(Prompt.None);
            var authorizeUrl = authorizeUriBuilder.Build();

            var result = await this.AttemptAuthorizeEndpointAsync(authorizeUrl, "openAuthorizationIframe");
            return result;
        }

        private async Task<AuthorizationCodeResult> AttemptPopupAsync(EndpointUriBuilder authorizeUriBuilder, GetTokenOptions getOptions)
        {
            authorizeUriBuilder.WithPrompt(getOptions.Prompt);
            var authorizeUrl = authorizeUriBuilder.Build();

            var result = await this.AttemptAuthorizeEndpointAsync(authorizeUrl, "openAuthorizationPopup");
            return result;
        }

        private async Task<AuthorizationCodeResult> AttemptAuthorizeEndpointAsync(string authorizeUrl, string jsFunction)
        {
            var result = new AuthorizationCodeResult();
            var input = new Dictionary<string, object>
            {
                { "authorizeUrl", authorizeUrl }
            };

            try
            {
                using (var handler = await this.ScriptService.CreateCallbackHandlerAsync<string>(jsFunction, data: input))
                {
                    var responseUrl = await handler.GetResultAsync(timeout: AuthorizeTimeout);
                    this.AugmentAuthorizationCodeResultFromResponseUrl(result, responseUrl);
                }
            }
            catch (FailureCallbackException ex)
            {
                var msg = ex.Message;
                var json = JsonSerializer.Serialize(ex.Result);
                try
                {
                    var popupResult = JsonSerializer.Deserialize<AuthorizationEndpointFailure>(json, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    result.FailureReason = popupResult?.Reason;
                }
                catch (Exception innerEx)
                {
                    var innerMsg = innerEx.Message;
                    result.FailureReason = AuthorizationCodeFailureReason.SystemFailure;
                }
            }
            catch (InteropTimeoutException ex)
            {
                var msg = ex.Message;
                result.FailureReason = AuthorizationCodeFailureReason.TimedOut;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                result.FailureReason = AuthorizationCodeFailureReason.SystemFailure;
            }

            if(result.FailureReason != null)
            {
                result.Code = null;
            }

            return result;
        }

        private void AugmentAuthorizationCodeResultFromResponseUrl(AuthorizationCodeResult result, string responseUrl)
        {
            if (responseUrl.Contains('?'))
            {
                var query = responseUrl.Substring(responseUrl.IndexOf('?') + 1);
                NameValueCollection queryParameters = System.Web.HttpUtility.ParseQueryString(query);
                string? error = this.GetValue(queryParameters, "error");
                string? code = this.GetValue(queryParameters, "code");

                if (error?.Length > 0)
                {
                    result.FailureReason = AuthorizationCodeFailureReason.IdPError;
                    result.ErrorCode = error;
                    result.ErrorDescription = this.GetValue(queryParameters, "error_description");
                    result.ErrorUri = this.GetValue(queryParameters, "error_uri");
                    result.Code = null;
                }
                else if (code?.Length > 0)
                {
                    result.Code = code;
                    result.ErrorCode = null;
                    result.ErrorDescription = null;
                    result.FailureReason = null;
                }
            }
        }

        private string? GetValue(NameValueCollection collection, string key)
        {
            if(collection.AllKeys.Contains(key))
            {
                return collection[key];
            }

            return null;
        }

        private string CreateNonce()
        {
            var data = new byte[32];
            RandomNumberGenerator.Fill(data);

            var base64 = Convert.ToBase64String(data);
            return base64
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

    }
}
