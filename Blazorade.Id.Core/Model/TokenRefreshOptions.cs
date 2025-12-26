using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Options for refreshing tokens.
    /// </summary>
    public class TokenRefreshOptions
    {
        /// <summary>
        /// The refresh token to use for refreshing.
        /// </summary>
        /// <remarks>
        /// If this property is set, the token refresh MUST use the given refresh token.
        /// If not specified, the token refresher may attempt to read the refresh token
        /// from a token store.
        /// </remarks>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// The scopes the tokens should be refreshed for.
        /// </summary>
        public IEnumerable<string> Scopes { get; set; } = [];
    }
}
