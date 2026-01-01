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
        public TestTokenRefresher(ITokenStore tokenStore, IRefreshTokenStore refreshTokenStore, IScopeAnalyzer scopeAnalyzer)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.RefreshTokenStore = refreshTokenStore ?? throw new ArgumentNullException(nameof(refreshTokenStore));
            this.ScopeAnalyzer = scopeAnalyzer ?? throw new ArgumentNullException(nameof(scopeAnalyzer));
        }

        private readonly ITokenStore TokenStore;
        private readonly IRefreshTokenStore RefreshTokenStore;
        private readonly IScopeAnalyzer ScopeAnalyzer;


        public DateTimeOffset? Expiration { get; set; }
        public string? RefreshToken { get; set; }

        public async Task<bool> RefreshTokensAsync(TokenRefreshOptions options, CancellationToken cancellationToken = default)
        {
            var refreshToken = this.RefreshToken ?? await this.RefreshTokenStore.GetRefreshTokenAsync();
            if (null == refreshToken) return false;

            var sorted = await this.ScopeAnalyzer.AnalyzeScopesAsync(options.Scopes, cancellationToken);
            foreach(var item in sorted)
            {
                var claims = new List<Claim>()
                {
                    new Claim("scp", string.Join(' ', from x in item.Value select x.Value))
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
                await this.RefreshTokenStore.SetRefreshTokenAsync("refreshed-refresh-token");
            }

            return sorted.Count > 0;
        }
    }
}
