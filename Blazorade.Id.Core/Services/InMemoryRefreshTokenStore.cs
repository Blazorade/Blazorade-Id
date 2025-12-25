using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A refresh token store that keeps the refresh tokn in memory.
    /// </summary>
    public class InMemoryRefreshTokenStore : IRefreshTokenStore
    {
        private TokenContainer? RefreshToken = null;

        /// <inheritdoc/>
        public Task ClearAsync()
        {
            this.RefreshToken = null;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<TokenContainer?> GetRefreshTokenAsync()
        {
            return Task.FromResult(this.RefreshToken);
        }

        /// <inheritdoc/>
        public Task SetRefreshTokenAsync(TokenContainer? token)
        {
            this.RefreshToken = token;
            return Task.CompletedTask;
        }
    }
}
