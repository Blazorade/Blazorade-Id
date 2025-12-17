using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The default token refresher provided by Blazorade Id.
    /// </summary>
    public class TokenRefresher : ITokenRefresher
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public TokenRefresher(
            IScopeSorter scopeSorter,
            ITokenStore tokenStore,
            IRedirectUriProvider redirectUriProvider,
            IOptions<AuthorityOptions> authOptions
        )
        {
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.RedirUriProvider = redirectUriProvider ?? throw new ArgumentNullException(nameof(redirectUriProvider));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly IScopeSorter ScopeSorter;
        private readonly ITokenStore TokenStore;
        private readonly IRedirectUriProvider RedirUriProvider;
        private readonly AuthorityOptions AuthOptions;

        /// <inheritdoc/>
        public async Task<bool> RefreshTokensAsync(TokenRefreshOptions options)
        {
            var refreshToken = await this.TokenStore.GetRefreshTokenAsync();
            if(refreshToken?.Token?.Length > 0)
            {
                var redirUri = this.AuthOptions.RedirectUri ?? this.RedirUriProvider.GetRedirectUri().ToString();
                var sortedScopes = this.ScopeSorter.SortScopes(options.Scopes);
                foreach (var item in sortedScopes)
                {
                    
                }
            }

            return false;
        }
    }
}
