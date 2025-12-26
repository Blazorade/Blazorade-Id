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
    /// Defines the interface that must be implemented by token store implementations.
    /// </summary>
    public interface ITokenStore
    {
        /// <summary>
        /// Clears all tokens from the token store.
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Returns the access token stored in the token store if it is available.
        /// </summary>
        /// <param name="resourceId">The identifier of the resource the access token is intended for.</param>
        Task<TokenContainer?> GetAccessTokenAsync(string resourceId);

        /// <summary>
        /// Returns the identity token stored in the token store if it is available.
        /// </summary>
        Task<TokenContainer?> GetIdentityTokenAsync();

        /// <summary>
        /// Stores the given access token container in the token store.
        /// </summary>
        /// <param name="resourceId">The identifier of the resource the access token is intended for.</param>
        /// <param name="token">The token container to store. If set to <see langword="null"/>, the implementation can either store the null value or remove it completely.</param>
        Task SetAccessTokenAsync(string resourceId, TokenContainer? token);

        /// <summary>
        /// Stores the given identity token contain in the token store.
        /// </summary>
        /// <param name="token">The token container to store. If set to <see langword="null"/>, the implementation can either store the null value or remove it completely.</param>
        Task SetIdentityTokenAsync(TokenContainer? token);

    }
}
