using Blazorade.Id.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Represents a request for tokens.
    /// </summary>
    public class TokenAcquisitionRequest
    {
        /// <summary>
        /// Specifies the mode how to acquire tokens.
        /// </summary>
        public TokenAcquisitionMode Mode { get; set; } = TokenAcquisitionMode.Silent;

        /// <summary>
        /// The scopes that the acquired access token must include.
        /// </summary>
        /// <remarks>
        /// The default value is defined by <see cref="AuthorityOptions.DefaultScope"/>.
        /// </remarks>
        public string Scope { get; set; } = AuthorityOptions.DefaultScope;

    }
}
