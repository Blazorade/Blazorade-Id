using Blazorade.Id.Configuration;
using Blazorade.Id.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Authentication service for Blazor applications.
    /// </summary>
    public class BlazorAuthenticationService : AuthenticationService
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public BlazorAuthenticationService(
            ITokenService tokenService,
            ITokenStore tokenStore, 
            IAuthenticationStateNotifier authStateNotifier, 
            IEndpointService endpointService, 
            NavigationManager navMan, 
            IOptions<AuthorityOptions> authOptions) : base(tokenService, tokenStore, authStateNotifier)
        {
            this.TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly ITokenService TokenService;
        private readonly ITokenStore TokenStore;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly IEndpointService EndpointService;
        private readonly NavigationManager NavMan;
        private readonly AuthorityOptions AuthOptions;


        /// <inheritdoc/>
        public async override Task SignOutAsync(SignOutOptions? options = null, CancellationToken cancellationToken = default)
        {
            options = options ?? new SignOutOptions { UseDefaultRedirectUri = true };
            options.RedirectUri = options.RedirectUri ?? (options.UseDefaultRedirectUri ? this.NavMan.BaseUri : null);

            var idToken = await this.TokenStore.GetIdentityTokenAsync();

            await this.TokenStore.ClearAllAsync();
            await this.AuthStateNotifier.StateHasChangedAsync();

            if (!options.SkipEndIdpSession)
            {
                var builder = await this.EndpointService.CreateEndSessionUriBuilderAsync();
                builder
                    .WithIdTokenHint(idToken?.Token)
                    .WithClientId(this.AuthOptions.ClientId)
                    .WithPostLogoutRedirectUri(options.RedirectUri);

                this.NavMan.NavigateTo(builder.Build());
            }
        }
    }
}
