using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Sign-out service for Blazor applications.
    /// </summary>
    public class BlazorSignOutService : ISignOutService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlazorSignOutService"/> class.
        /// </summary>
        public BlazorSignOutService(ITokenStore tokenStore, IAuthenticationStateNotifier authStateNotifier, IEndpointService endpointService, NavigationManager navMan, IOptions<AuthorityOptions> authOptions)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.EndpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly ITokenStore TokenStore;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly IEndpointService EndpointService;
        private readonly NavigationManager NavMan;
        private readonly AuthorityOptions AuthOptions;

        /// <inheritdoc/>
        public async Task SignOutAsync(SignOutOptions? options = null)
        {
            options = options ?? new SignOutOptions();
            options.RedirectUri = options.RedirectUri ?? (options.UseDefaultRedirectUri ? this.NavMan.Uri : null);

            var idToken = await this.TokenStore.GetIdentityTokenAsync();

            var builder = await this.EndpointService.CreateEndSessionUriBuilderAsync();
            builder
                .WithIdTokenHint(idToken?.Token)
                .WithClientId(this.AuthOptions.ClientId)
                .WithPostLogoutRedirectUri(options.RedirectUri)
                ;

            await this.TokenStore.ClearAllAsync();
            await this.AuthStateNotifier.StateHasChangedAsync();

            this.NavMan.NavigateTo(builder.Build());
        }
    }
}
