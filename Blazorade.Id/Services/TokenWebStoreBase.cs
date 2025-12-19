using Blazorade.Id.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// The base class for token stores that operate in web-based environments,
    /// i.e. session storage or local storage.
    /// </summary>
    public abstract class TokenWebStoreBase : TokenStoreBase
    {

        /// <summary>
        /// Gets or sets a value indicating whether refresh tokens are permitted to be stored in web storage, such as
        /// localStorage or sessionStorage.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, this option is set to <see langword="false"/>. If so, refresh tokens will be directed into
        /// the <see cref="InMemoryTokenStore"/>. If you set this option to <see langword="true"/>, refresh tokens
        /// will be stored in the web storage mechanism implemented by the derived class.
        /// </para>
        /// <para>
        /// WARNING! Enabling this option allows refresh tokens to be stored in browser session storage or local 
        /// storage. These storage mechanisms are accessible to any JavaScript running in the application’s origin 
        /// and must be considered compromised in the presence of an XSS vulnerability or malicious browser 
        /// extension. Refresh tokens are long-lived, high-value credentials; if exfiltrated, they can be reused 
        /// outside the browser until revoked or expired. Only enable this setting if you fully understand and accept 
        /// the increased risk.
        /// </para>
        /// </remarks>
        public bool AllowRefreshTokensInWebStorage { get; set; } = false;



        /// <summary>
        /// An in-memory token store used for storing tokens that should not be stored in web storage.
        /// </summary>
        protected InMemoryTokenStore InMemoryStore { get; } = new InMemoryTokenStore();

    }
}
