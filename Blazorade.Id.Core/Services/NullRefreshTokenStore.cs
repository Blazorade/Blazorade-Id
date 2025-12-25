using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// The default refresh token store implementation. This implementation does not store refresh tokens at all.
    /// </summary>
    public class NullRefreshTokenStore : IRefreshTokenStore
    {
        /// <inheritdoc/>
        public Task ClearAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<TokenContainer?> GetRefreshTokenAsync()
        {
            return Task.FromResult<TokenContainer?>(null);
        }

        /// <inheritdoc/>
        public Task SetRefreshTokenAsync(TokenContainer? token)
        {
            return Task.CompletedTask;
        }
    }
}
