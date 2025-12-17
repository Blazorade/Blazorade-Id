using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Defines the interface that must be implemented by token refresher implementations.
    /// </summary>
    /// <remarks>
    /// A token refresher is responsible for refreshing tokens using a refresh token when the access token has expired.
    /// </remarks>
    public interface ITokenRefresher
    {
        /// <summary>
        /// Refreshes tokens using the given options.
        /// </summary>
        /// <param name="options">The options to use when refreshing tokens.</param>
        /// <returns>Returns <see langword="true"/> if the refresh was successful. <see langword="false"/> otherwise.</returns>
        Task<bool> RefreshTokensAsync(TokenRefreshOptions options);
    }
}
