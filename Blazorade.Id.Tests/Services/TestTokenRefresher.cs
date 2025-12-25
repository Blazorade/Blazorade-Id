using Blazorade.Id.Model;
using Blazorade.Id.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    public class TestTokenRefresher : ITokenRefresher
    {
        public TestTokenRefresher(ITokenStore tokenStore, IScopeSorter scopeSorter)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.ScopeSorter = scopeSorter ?? throw new ArgumentNullException(nameof(scopeSorter));
        }

        private readonly ITokenStore TokenStore;
        private readonly IScopeSorter ScopeSorter;


        public DateTimeOffset? Expiration { get; set; }
        public TokenContainer? RefreshToken { get; set; }

        public async Task<bool> RefreshTokensAsync(TokenRefreshOptions options)
        {
            var refreshToken = this.RefreshToken ?? await this.TokenStore.GetRefreshTokenAsync();
            if (null == refreshToken) return false;

            var sorted = await this.ScopeSorter.SortScopesAsync(options.Scopes);
            foreach(var item in sorted)
            {
                var claims = new List<Claim>()
                {
                    new Claim("scp", string.Join(' ', item.Value))
                };

                if(this.Expiration.HasValue)
                {
                    claims.Add(new Claim("exp", this.Expiration.Value.ToUnixTimeSeconds().ToString()));
                }

                var token = new JwtSecurityToken(
                    claims: claims
                );

                
                var container = new TokenContainer(token);
                await this.TokenStore.SetAccessTokenAsync(item.Key, container);
            }

            if(sorted.Count > 0)
            {
                await this.TokenStore.SetRefreshTokenAsync("refreshed-refresh-token");
            }

            return sorted.Count > 0;
        }
    }
}
