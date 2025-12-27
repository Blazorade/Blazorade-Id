using Blazorade.Id.Configuration;
using Blazorade.Id.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
            IRefreshTokenStore refreshTokenStore,
            ITokenStore tokenStore, 
            IAuthenticationStateNotifier authStateNotifier, 
            IEndpointService endpointService, 
            NavigationManager navMan, 
            IJSRuntime jsRuntime,
            IOptions<AuthorityOptions> authOptions,
            IOptions<JsonSerializerOptions> jsonOptions) : base(tokenService, tokenStore, refreshTokenStore, authStateNotifier)
        {
            this.TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.RefreshTokenStore = refreshTokenStore ?? throw new ArgumentNullException(nameof(refreshTokenStore));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.SessionPropertyStore = new BrowserSessionStoragePropertyStore(jsRuntime, jsonOptions);
            this.LocalPropertyStore = new BrowserLocalStoragePropertyStore(jsRuntime, jsonOptions);
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly ITokenService TokenService;
        private readonly ITokenStore TokenStore;
        private readonly IRefreshTokenStore RefreshTokenStore;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly IEndpointService EndpointService;
        private readonly NavigationManager NavMan;
        private readonly IPropertyStore SessionPropertyStore;
        private readonly IPropertyStore LocalPropertyStore;
        private readonly AuthorityOptions AuthOptions;

        /// <inheritdoc/>
        public async override Task SignOutAsync(SignOutOptions? options = null, CancellationToken cancellationToken = default)
        {
            options = options ?? new SignOutOptions { UseDefaultRedirectUri = true };
            options.RedirectUri = options.RedirectUri ?? (options.UseDefaultRedirectUri ? this.NavMan.BaseUri : null);

            var idToken = await this.TokenStore.GetIdentityTokenAsync();

            await this.RefreshTokenStore.ClearAsync();
            await this.TokenStore.ClearAsync();
            await this.AuthStateNotifier.StateHasChangedAsync();
            await this.SessionPropertyStore.RemoveAllAsync();
            await this.LocalPropertyStore.RemoveAllAsync();

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
