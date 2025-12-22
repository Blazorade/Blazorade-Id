using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    ///  A token store implementation that keeps tokens in memory.
    /// </summary>
    public class InMemoryTokenStore : TokenStoreBase
    {
        private Dictionary<string, TokenContainer> AccessTokens = new Dictionary<string, TokenContainer>();
        private TokenContainer? IdentityToken;
        private TokenContainer? RefreshToken;

        /// <inheritdoc/>
        public override Task ClearAllAsync()
        {
            this.AccessTokens.Clear();
            this.IdentityToken = null;
            this.RefreshToken = null;

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task<TokenContainer?> GetAccessTokenAsync(string resourceId)
        {
            TokenContainer? container = null;
            if(this.AccessTokens.ContainsKey(resourceId))
            {
                container = this.AccessTokens[resourceId];
                if(container.Expires < DateTime.UtcNow)
                {
                    container = null;
                    this.AccessTokens.Remove(resourceId);
                }
            }

            return Task.FromResult<TokenContainer?>(container);
        }

        /// <inheritdoc/>
        public override Task<TokenContainer?> GetIdentityTokenAsync()
        {
            if(this.IdentityToken?.Expires > DateTime.UtcNow)
            {
                return Task.FromResult<TokenContainer?>(this.IdentityToken);
            }

            return Task.FromResult<TokenContainer?>(null);
        }

        /// <inheritdoc/>
        public override Task<TokenContainer?> GetRefreshTokenAsync()
        {
            if(null == this.RefreshToken?.Expires || this.RefreshToken?.Expires > DateTime.UtcNow)
            {
                return Task.FromResult<TokenContainer?>(this.RefreshToken);
            }

            return Task.FromResult<TokenContainer?>(null);
        }

        /// <inheritdoc/>
        public override Task SetAccessTokenAsync(string resourceId, TokenContainer? token)
        {
            if(null != token)
            {
                this.AccessTokens[resourceId] = token;
            }
            else if(this.AccessTokens.ContainsKey(resourceId))
            {
                this.AccessTokens.Remove(resourceId);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task SetIdentityTokenAsync(TokenContainer? token)
        {
            this.IdentityToken = token;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task SetRefreshTokenAsync(TokenContainer? token)
        {
            if(this.StoreRefreshTokens)
            {
                this.RefreshToken = token;
            }
            return Task.CompletedTask;
        }
    }
}
