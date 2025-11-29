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
            IOptions<AuthorityOptions> authOptions
        ) {
            this.JsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
            this.CodeChallengeService = codeChallengeService ?? throw new ArgumentNullException(nameof(codeChallengeService));
            this.PropertyStore = propertyStore ?? throw new ArgumentNullException(nameof(propertyStore));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
        }

        private readonly IJSRuntime JsRuntime;
        private readonly EndpointService EndpointService;
        private readonly ICodeChallengeService CodeChallengeService;
        private readonly IPropertyStore PropertyStore;
        private readonly NavigationManager NavMan;
        private readonly AuthorityOptions AuthOptions;

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
                .WithRedirectUri(redirUrl)
                .WithScope(string.Join(' ', scopes))
                .WithCodeChallenge(codeVerifier)
                .WithPrompt(Prompt.Select_Account)

                .Build();

            await this.JsRuntime.InvokeVoidAsync("open", uri, "blazoradeauth", "scrollbars=no,resizable=no,status=no,location=no,toolbar=no,menubar=no,width=480,height=640,left=200,top=200");
            return null;
        }
    }
}
