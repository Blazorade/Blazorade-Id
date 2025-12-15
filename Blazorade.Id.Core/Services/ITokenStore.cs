using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public interface ITokenStore
    {

        /// <summary>
        /// Returns the access token stored in the token store if it is available and if it is still valid.
        /// </summary>
        /// <returns></returns>
        ValueTask<TokenContainer?> GetAccessTokenAsync();

        /// <summary>
        /// Returns the scopes that a previously acquired token was acquired with.
        /// </summary>
        ValueTask<string?> GetAcquiredScopesAsync();

        /// <summary>
        /// Returns the identity token stored in the token store if it is available and if it is still valid.
        /// </summary>
        /// <returns></returns>
        ValueTask<TokenContainer?> GetIdentityTokenAsync();

        /// <summary>
        /// Returns the refresh token stored in the token store if it is available.
        /// </summary>
        /// <returns></returns>
        ValueTask<TokenContainer?> GetRefreshTokenAsync();

        /// <summary>
        /// Sets the scopes that a previously acquired token was acquired with.
        /// </summary>
        ValueTask SetAcquiredScopesAsync(string scopes);

        /// <summary>
        /// Stores the given access token container in the token store.
        /// </summary>
        /// <param name="token">The token container to store.</param>
        ValueTask SetAccessTokenAsync(TokenContainer token);

        /// <summary>
        /// Stores the given identity token contain in the token store.
        /// </summary>
        /// <param name="token">The token container to store.</param>
        /// <returns></returns>
        ValueTask SetIdentityTokenAsync(TokenContainer token);

        /// <summary>
        /// Stores the given refresh token container in the token store.
        /// </summary>
        /// <param name="token">The refresh token to store.</param>
        ValueTask SetRefreshTokenAsync(TokenContainer token);
    }
}
