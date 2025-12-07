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
        public abstract ValueTask<TokenContainer?> GetIdentityTokenAsync();

        /// <inheritdoc/>
        public abstract ValueTask<TokenContainer?> GetRefreshTokenAsync();

        /// <inheritdoc/>
        public async virtual ValueTask<TokenContainer?> SetAccessTokenAsync(string token)
        {
            var jwt = new JwtSecurityToken(token);
            return await this.SetAccessTokenAsync(jwt);
        }

        /// <inheritdoc/>
        public async ValueTask<TokenContainer?> SetAccessTokenAsync(JwtSecurityToken token)
        {
            var container = new TokenContainer(token);
            await this.SetAccessTokenAsync(container);
            return container;
        }

        /// <inheritdoc/>
        public abstract ValueTask SetAccessTokenAsync(TokenContainer token);

        /// <inheritdoc/>
        public async ValueTask<TokenContainer?> SetIdentityTokenAsync(string token)
        {
            var jwt = new JwtSecurityToken(token);
            return await this.SetIdentityTokenAsync(jwt);
        }

        /// <inheritdoc/>
        public async ValueTask<TokenContainer?> SetIdentityTokenAsync(JwtSecurityToken token)
        {
            var container = new TokenContainer(token);
            await this.SetIdentityTokenAsync(container);
            return container;
        }

        /// <inheritdoc/>
        public abstract ValueTask SetIdentityTokenAsync(TokenContainer token);

        /// <inheritdoc/>
        public virtual async ValueTask<TokenContainer?> SetRefreshTokenAsync(string token)
        {
            var container = new TokenContainer(token);
            await this.SetRefreshTokenAsync(container);
            return container;
        }

        /// <inheritdoc/>
        public abstract ValueTask SetRefreshTokenAsync(TokenContainer token);



        /// <summary>
        /// Returns a key that can be used to store/retrieve a token of the specified <paramref name="tokenType"/>.
        /// </summary>
        protected string GetKey(TokenType tokenType)
        {
            return $"blazorade.id.{tokenType.ToString().ToLower()}";
        }

    }
}
