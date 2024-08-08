using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Defines various grant types.
    /// </summary>
    public enum GrantType
    {
        /// <summary>
        /// The request is made using an authorization code obtained through the authorization code flow.
        /// </summary>
        Authorization_Code,

        /// <summary>
        /// The request is made using a refresh token.
        /// </summary>
        Refresh_Token
    }
}
