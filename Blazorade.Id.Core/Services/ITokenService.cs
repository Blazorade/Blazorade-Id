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
    /// The service interface for a token service. A token service is responsible for retrieving tokens such as access tokens and identity tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Returns zero or more access tokens that match the specified <paramref name="options"/>.
        /// </summary>
        /// <remarks>
        /// If a valid token is not available, the token service is responsible for either refreshing the token using a token refresher service
        /// or starting a new authorization flow to obtain a new access token.
        /// </remarks>
        Task<AccessTokenDictionary> GetAccessTokensAsync(GetTokenOptions? options = null);

        /// <summary>
        /// Returns the identity token for the current user.
        /// </summary>
        /// <remarks>
        /// If a valid token is not available, the token service is responsible for either refreshing the token using a refresh token or starting
        /// a new authorization flow to obtain a new identity token.
        /// </remarks>
        Task<JwtSecurityToken?> GetIdentityTokenAsync(GetTokenOptions? options = null);
    }
}
