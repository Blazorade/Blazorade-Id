using Blazorade.Id.Model;
using Blazorade.Id.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Tests.Services
{
    public class TestCodeProcessor : IAuthorizationCodeProcessor
    {
        public TestCodeProcessor(ITokenStore tokenStore, IRefreshTokenStore refreshTokenStore, IScopeSorter scopeSorter)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.TokenRefresher = new TestTokenRefresher(this.TokenStore, refreshTokenStore, scopeSorter);
            this.TokenRefresher.RefreshToken = "refresh-token-from-code-processor";
        }

        private readonly ITokenStore TokenStore;
        private readonly TestTokenRefresher TokenRefresher;

        public string[] ScopesToProcess { get; set; } = [];

        public async Task<bool> ProcessAuthorizationCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            await this.TokenRefresher.RefreshTokensAsync(new TokenRefreshOptions
            {
                Scopes = this.ScopesToProcess
            });

            return await Task.FromResult(true);
        }
    }
}
