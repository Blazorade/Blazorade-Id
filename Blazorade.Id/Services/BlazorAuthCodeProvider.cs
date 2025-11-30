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

namespace Blazorade.Id.Services
{
    public class BlazorAuthCodeProvider : IAuthCodeProvider
    {
        public BlazorAuthCodeProvider(
            IJSRuntime jsRuntime, 
            EndpointService endpointService, 
            ICodeChallengeService codeChallengeService,
            IPropertyStore propertyStore,
            NavigationManager navMan,
            BlazoradeIdScriptService scriptService,
            IOptions<AuthorityOptions> authOptions
        ) {
            this.JsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.CodeChallengeService = codeChallengeService ?? throw new ArgumentNullException(nameof(codeChallengeService));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.ScriptService = scriptService ?? throw new ArgumentNullException(nameof(scriptService));
        }

        private readonly IJSRuntime JsRuntime;
        private readonly EndpointService EndpointService;
        private readonly ICodeChallengeService CodeChallengeService;
        private readonly IPropertyStore PropertyStore;
        private readonly NavigationManager NavMan;
        private readonly AuthorityOptions AuthOptions;
        private readonly BlazoradeIdScriptService ScriptService;

        private const int AuthorizeTimeout = 60000;

        public async Task<string?> GetAuthorizationCodeAsync(IEnumerable<string> scopes)
        {
            var redirUrl = this.AuthOptions.RedirectUri?.Length > 0
                ? this.AuthOptions.RedirectUri
                : new Uri(new Uri(this.NavMan.BaseUri), OAuthCallback.RoutePath).ToString();

            var codeVerifier = this.CodeChallengeService.CreateCodeVerifier();
            await this.PropertyStore.SetCodeVerifierAsync(codeVerifier);

            var uriBuilder = await this.EndpointService.CreateAuthorizationUriBuilderAsync();
            var uri = uriBuilder
                .WithClientId(this.AuthOptions.ClientId)
                .WithResponseType(ResponseType.Code)
                .WithResponseMode(ResponseMode.Query)
                .WithRedirectUri(redirUrl)
                .WithScope(string.Join(' ', scopes))
                .WithCodeChallenge(codeVerifier)
                .WithPrompt(Prompt.Select_Account)

                .Build();

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

    }
}
