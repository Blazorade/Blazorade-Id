using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Defines the service contract for refresh token stores.
    /// </summary>
    public interface IRefreshTokenStore
    {
        /// <summary>
        /// Clears the token from the token store.
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Returns the refresh token stored in the token store if it is available.
        /// </summary>
        Task<string?> GetRefreshTokenAsync();

        /// <summary>
        /// Stores the given refresh token container in the token store.
        /// </summary>
        /// <param name="token">The token to store. If set to <see langword="null"/>, the implementation can either store the null value or remove it completely.</param>
        Task SetRefreshTokenAsync(string? token);
    }
}
