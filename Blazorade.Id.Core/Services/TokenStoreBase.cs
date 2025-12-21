using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A base implementation that can be used as base class for token store implementations.
    /// </summary>
    public abstract class TokenStoreBase : ITokenStore
    {

        /// <inheritdoc/>
        public abstract Task ClearAllAsync();

        /// <inheritdoc/>
        public abstract Task<TokenContainer?> GetAccessTokenAsync(string resourceId);

        /// <inheritdoc/>
        public abstract Task<TokenContainer?> GetIdentityTokenAsync();

        /// <inheritdoc/>
        public abstract Task<TokenContainer?> GetRefreshTokenAsync();

        /// <inheritdoc/>
        public abstract Task SetAccessTokenAsync(string resourceId, TokenContainer? token);

        /// <inheritdoc/>
        public abstract Task SetIdentityTokenAsync(TokenContainer? token);

        /// <inheritdoc/>
        public abstract Task SetRefreshTokenAsync(TokenContainer? token);



        /// <summary>
        /// Returns a key that can be used to store/retrieve a token of the specified <paramref name="tokenType"/>.
        /// </summary>
        protected string GetKey(TokenType tokenType, string? suffix = null)
        {
            return this.GetKey(tokenType.ToString(), suffix: suffix);
        }

        /// <summary>
        /// Returns the prefix for all keys stored by Blazorade Id token store implementations.
        /// </summary>
        /// <returns></returns>
        protected string GetKeyPrefix()
        {
            return "blazorade.id.";
        }

        /// <summary>
        /// Returns a fully qualified key for the specified <paramref name="name"/>.
        /// </summary>
        protected string GetKey(string name, string? suffix = null)
        {
            var prefix = this.GetKeyPrefix();

            suffix = null != suffix ? $".{suffix}" : string.Empty;
            return $"{prefix}{name.ToLower()}{suffix}";
        }
    }
}
