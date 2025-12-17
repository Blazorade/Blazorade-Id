using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
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

        public bool DisableRefresh { get; set; } = false;

        public async Task<bool> RefreshTokensAsync(TokenRefreshOptions options)
        {
            if (DisableRefresh) return false;

            var sorted = this.ScopeSorter.SortScopes(options.Scopes);
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

                
                var container = new TokenContainer(token, new GetTokenOptions { Scopes = item.Value.Select(x => x.ToString()) });
                await this.TokenStore.SetAccessTokenAsync(item.Key, container);
            }

            return sorted.Count > 0;
        }
    }
}
