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

            var uri = uriBuilder.Build();
            string? code = null;
            var lastSuccessfulTimestamp = await this.LocalStore.GetLastSuccessfulAuthCodeTimestampAsync();

            AuthorizationCallbackResult callbackResult = null != lastSuccessfulTimestamp && !options.Prompt.RequiresInteraction()
                ? await this.AttemptIFrameAsync(uri)
                : new AuthorizationCallbackResult();

            if (string.IsNullOrEmpty(callbackResult.ResponseUrl) || callbackResult.FailureReason != null)
            {
                // IFrame attempt failed, was not attempted, or authentication has never succeeded before, try popup.
                callbackResult = await this.AttemptPopupAsync(uri);
            }

            if (callbackResult.ResponseUrl?.Length > 0 && callbackResult.ResponseUrl.Contains('?') && null == callbackResult.FailureReason)
            {
                var query = callbackResult.ResponseUrl.Substring(callbackResult.ResponseUrl.IndexOf('?') + 1);
                var queryParameters = System.Web.HttpUtility.ParseQueryString(query);
                if (queryParameters.AllKeys.Contains("code"))
                {
                    code = queryParameters["code"];
                }
            }

            var result = new AuthorizationCodeResult
            {
                Code = code,
                FailureReason = code?.Length > 0 ? null : callbackResult.FailureReason
            };

            if(result.Code?.Length > 0 && result.FailureReason == null)
            {
                await this.LocalStore.SetLastSuccessfulAuthCodeTimestampAsync(DateTime.UtcNow);
            }

            return result;
        }


        private async Task<AuthorizationCallbackResult> AttemptIFrameAsync(string authorizeUrl)
        {
            var result = await this.AttemptAuthorizeEndpointAsync(authorizeUrl, "openAuthorizationIframe");
            return result;
        }

        private async Task<AuthorizationCallbackResult> AttemptPopupAsync(string authorizeUrl)
        {
            var result = await this.AttemptAuthorizeEndpointAsync(authorizeUrl, "openAuthorizationPopup");
            return result;
        }

        private async Task<AuthorizationCallbackResult> AttemptAuthorizeEndpointAsync(string authorizeUrl, string jsFunction)
        {
            var result = new AuthorizationCallbackResult();
            var input = new Dictionary<string, object>
            {
                { "authorizeUrl", authorizeUrl }
            };

            try
            {
                using (var handler = await this.ScriptService.CreateCallbackHandlerAsync<string>(jsFunction, data: input))
                {
                    result.ResponseUrl = await handler.GetResultAsync(timeout: AuthorizeTimeout);
                    result.FailureReason = null;
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

            return result;
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


        private class AuthorizationCallbackResult
        {
            public string? ResponseUrl { get; set; }

            public AuthorizationCodeFailureReason? FailureReason { get; set; }
        }
    }
}
