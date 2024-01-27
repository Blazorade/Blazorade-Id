using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public enum ResponseType
    {
        /// <summary>
        /// Authorization code.
        /// </summary>
        Code,

        /// <summary>
        /// ID token.
        /// </summary>
        Id_Token,

        /// <summary>
        /// Access token.
        /// </summary>
        Token
    }
}
