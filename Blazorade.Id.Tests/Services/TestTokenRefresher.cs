using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<bool> RefreshTokensAsync(TokenRefreshOptions options)
        {
            var sorted = this.ScopeSorter.SortScopes(options.Scopes);
            foreach(var item in sorted)
            {
                
            }

            return Task.FromResult(true);
        }
    }
}
