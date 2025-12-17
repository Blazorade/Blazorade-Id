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
    /// Defines the interface that must be implemented by token store implementations.
    /// </summary>
    public interface ITokenStore
    {

        /// <summary>
        /// Returns the access token stored in the token store if it is available and if it is still valid.
        /// </summary>
        /// <param name="resourceId">The identifier of the resource the access token is intended for.</param>
        ValueTask<TokenContainer?> GetAccessTokenAsync(string resourceId);

        /// <summary>
        /// Returns the identity token stored in the token store if it is available and if it is still valid.
        /// </summary>
        ValueTask<TokenContainer?> GetIdentityTokenAsync();

        /// <summary>
        /// Returns the refresh token stored in the token store if it is available.
        /// </summary>
        ValueTask<TokenContainer?> GetRefreshTokenAsync();

        /// <summary>
        /// Stores the given access token container in the token store.
        /// </summary>
        /// <param name="resourceId">The identifier of the resource the access token is intended for.</param>
        /// <param name="token">The token container to store.</param>
        ValueTask SetAccessTokenAsync(string resourceId, TokenContainer token);

        /// <summary>
        /// Stores the given identity token contain in the token store.
        /// </summary>
        /// <param name="token">The token container to store.</param>
        ValueTask SetIdentityTokenAsync(TokenContainer token);

        /// <summary>
        /// Stores the given refresh token container in the token store.
        /// </summary>
        /// <param name="token">The refresh token to store.</param>
        ValueTask SetRefreshTokenAsync(TokenContainer token);
    }
}
