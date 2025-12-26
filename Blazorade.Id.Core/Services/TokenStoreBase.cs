using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A base implementation that can be used as base class for token store implementations.
    /// </summary>
    public abstract class TokenStoreBase : StoreBase, ITokenStore
    {


        /// <inheritdoc/>
        public abstract Task ClearAsync();

        /// <inheritdoc/>
        public abstract Task<TokenContainer?> GetAccessTokenAsync(string resourceId);

        /// <inheritdoc/>
        public abstract Task<TokenContainer?> GetIdentityTokenAsync();

        /// <inheritdoc/>
        public abstract Task SetAccessTokenAsync(string resourceId, TokenContainer? token);

        /// <inheritdoc/>
        public abstract Task SetIdentityTokenAsync(TokenContainer? token);



    }
}
