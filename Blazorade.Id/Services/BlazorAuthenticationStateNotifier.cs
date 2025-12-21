using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Notifies the Blazor authentication state provider about changes in authentication state.
    /// </summary>
    public class BlazorAuthenticationStateNotifier : IAuthenticationStateNotifier
    {
        /// <summary>
        /// Creates a new instance of the service implementation.
        /// </summary>
        /// <exception cref="ArgumentNullException">The exception that is thrown when any of the parameters are null.</exception>
        public BlazorAuthenticationStateNotifier(IHostEnvironmentAuthenticationStateProvider authStateProvider, ITokenStore tokenStore)
        {
            this.AuthStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        }

        private readonly IHostEnvironmentAuthenticationStateProvider AuthStateProvider;
        private readonly ITokenStore TokenStore;

        /// <inheritdoc/>
        public async Task StateHasChangedAsync()
        {
            var principal = await this.CreatePrincipalAsync() ?? new ClaimsPrincipal();
            this.AuthStateProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
        }



        private async Task<ClaimsPrincipal?> CreatePrincipalAsync()
        {
            var idTokenContainer = await this.TokenStore.GetIdentityTokenAsync();
            if(null != idTokenContainer)
            {
                var idToken = idTokenContainer.ParseToken();
                if(null != idToken)
                {
                    return new ClaimsPrincipal(new ClaimsIdentity(idToken.Claims, "oidc", "name", "roles"));
                }
            }

            return null;
        }
    }
}
