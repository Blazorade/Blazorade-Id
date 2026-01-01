using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// The default implementation of the <see cref="IAuthenticationService"/> service interface.
    /// </summary>
    internal class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public AuthenticationService(ITokenService tokenService, ITokenStore tokenStore, IRefreshTokenStore refreshTokenStore, IAuthenticationStateNotifier authenticationStateNotifier)
        {
            this.TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.RefreshTokenStore = refreshTokenStore ?? throw new ArgumentNullException(nameof(refreshTokenStore));
            this.AuthenticationStateNotifier = authenticationStateNotifier ?? throw new ArgumentNullException(nameof(authenticationStateNotifier));
        }

        private readonly ITokenService TokenService;
        private readonly ITokenStore TokenStore;
        private readonly IRefreshTokenStore RefreshTokenStore;
        private readonly IAuthenticationStateNotifier AuthenticationStateNotifier;

        /// <inheritdoc/>
        public virtual async Task<ClaimsPrincipal?> SignInAsync(SignInOptions? options = null, CancellationToken cancellationToken = default)
        {
            ClaimsPrincipal? principal = null;
            options = options ?? new SignInOptions();
            
            var idToken = await this.TokenService.GetIdentityTokenAsync(options: options.ToGetTokenOptions(), cancellationToken);
            if(null != idToken)
            {
                principal = new ClaimsPrincipal(new ClaimsIdentity(idToken.Claims));
            }
            await this.AuthenticationStateNotifier.StateHasChangedAsync();

            return principal;
        }

        /// <inheritdoc/>
        public virtual async Task SignOutAsync(SignOutOptions? options = null, CancellationToken cancellationToken = default)
        {
            await this.RefreshTokenStore.ClearAsync();
            await this.TokenStore.ClearAsync();
            await this.AuthenticationStateNotifier.StateHasChangedAsync();
        }
    }
}
