using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Defines an enumeration of token types.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Represents an access token.
        /// </summary>
        AccessToken,
        /// <summary>
        /// Represents an identity token.
        /// </summary>
        IdentityToken,
        /// <summary>
        /// Represents a refresh token.
        /// </summary>
        RefreshToken
    }
}
