using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
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
        public override ValueTask<TokenContainer?> GetAccessTokenAsync(string resourceId)
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

            return ValueTask.FromResult<TokenContainer?>(container);
        }

        /// <inheritdoc/>
        public override ValueTask<TokenContainer?> GetIdentityTokenAsync()
        {
            if(this.IdentityToken?.Expires > DateTime.UtcNow)
            {
                return ValueTask.FromResult<TokenContainer?>(this.IdentityToken);
            }

            return ValueTask.FromResult<TokenContainer?>(null);
        }

        /// <inheritdoc/>
        public override ValueTask<TokenContainer?> GetRefreshTokenAsync()
        {
            if(null == this.RefreshToken?.Expires || this.RefreshToken?.Expires > DateTime.UtcNow)
            {
                return ValueTask.FromResult<TokenContainer?>(this.RefreshToken);
            }

            return ValueTask.FromResult<TokenContainer?>(null);
        }

        /// <inheritdoc/>
        public override ValueTask SetAccessTokenAsync(string resourceId, TokenContainer? token)
        {
            if(null != token)
            {
                this.AccessTokens[resourceId] = token;
            }
            else if(this.AccessTokens.ContainsKey(resourceId))
            {
                this.AccessTokens.Remove(resourceId);
            }

            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public override ValueTask SetIdentityTokenAsync(TokenContainer? token)
        {
            this.IdentityToken = token;
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public override ValueTask SetRefreshTokenAsync(TokenContainer? token)
        {
            this.RefreshToken = token;
            return ValueTask.CompletedTask;
        }
    }
}
