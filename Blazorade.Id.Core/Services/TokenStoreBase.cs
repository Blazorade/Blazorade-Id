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
        public abstract ValueTask<TokenContainer?> GetAccessTokenAsync();

        /// <inheritdoc/>
        public abstract ValueTask<string?> GetAcquiredScopesAsync();

        /// <inheritdoc/>
        public abstract ValueTask<TokenContainer?> GetIdentityTokenAsync();

        /// <inheritdoc/>
        public abstract ValueTask<TokenContainer?> GetRefreshTokenAsync();

        /// <inheritdoc/>
        public abstract ValueTask SetAccessTokenAsync(TokenContainer token);

        /// <inheritdoc/>
        public abstract ValueTask SetAcquiredScopesAsync(string scopes);

        /// <inheritdoc/>
        public abstract ValueTask SetIdentityTokenAsync(TokenContainer token);

        /// <inheritdoc/>
        public abstract ValueTask SetRefreshTokenAsync(TokenContainer token);



        /// <summary>
        /// Returns a key that can be used to store/retrieve a token of the specified <paramref name="tokenType"/>.
        /// </summary>
        protected string GetKey(TokenType tokenType)
        {
            return this.GetKey(tokenType.ToString());
        }

        /// <summary>
        /// Returns a fully qualified key for the specified <paramref name="name"/>.
        /// </summary>
        protected string GetKey(string name)
        {
            return $"blazorade.id.{name.ToLower()}";
        }
    }
}
