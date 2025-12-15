using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
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

namespace Blazorade.Id.Services
{
    /// <summary>
    /// An <see cref="IAuthCodeProvider"/> implementation for use in Blazor Server and Blazor WebAssembly applications.
    /// </summary>
    public class BlazorAuthCodeProvider : IAuthCodeProvider
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public BlazorAuthCodeProvider(
            EndpointService endpointService, 
            ICodeChallengeService codeChallengeService,
            IPropertyStore propertyStore,
            NavigationManager navMan,
            BlazoradeIdScriptService scriptService,
            IRedirectUriProvider redirUriProvider,
            IOptions<AuthorityOptions> authOptions
        ) {
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.CodeChallengeService = codeChallengeService ?? throw new ArgumentNullException(nameof(codeChallengeService));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.ScriptService = scriptService ?? throw new ArgumentNullException(nameof(scriptService));
            this.RedirUriProvider = redirUriProvider ?? throw new ArgumentNullException(nameof(redirUriProvider));
        }

        private readonly EndpointService EndpointService;
        private readonly ICodeChallengeService CodeChallengeService;
        private readonly IPropertyStore PropertyStore;
        private readonly NavigationManager NavMan;
        private readonly AuthorityOptions AuthOptions;
        private readonly BlazoradeIdScriptService ScriptService;
        private readonly IRedirectUriProvider RedirUriProvider;

        private const int AuthorizeTimeout = 60000;

        /// <inheritdoc/>
        public async Task<string?> GetAuthorizationCodeAsync(GetTokenOptions options)
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

            var uriBuilder = await this.EndpointService.CreateAuthorizationUriBuilderAsync();
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
            string responseUrl = string.Empty;
            string? code = null;
            var input = new Dictionary<string, object>
            {
                { "authorizeUrl", uri }
            };
            try
            {
                using (var handler = await this.ScriptService.CreateCallbackHandlerAsync<string>("openAuthorizationPopup", data: input))
                {
                    responseUrl = await handler.GetResultAsync(timeout: AuthorizeTimeout);
                }
            }
            catch (FailureCallbackException ex)
            {
                var msg = ex.Message;
                var result = ex.Result;
            }
            catch(InteropTimeoutException ex)
            {
                var msg = ex.Message;
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
            }

            if(responseUrl?.Length > 0 && responseUrl.Contains('?'))
            {
                var query = responseUrl.Substring(responseUrl.IndexOf('?') + 1);
                var queryParameters = System.Web.HttpUtility.ParseQueryString(query);
                if (queryParameters.AllKeys.Contains("code"))
                {
                    code = queryParameters["code"];
                }
            }

            return code;
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
