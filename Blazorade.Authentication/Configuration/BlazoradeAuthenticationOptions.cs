using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Authentication.Configuration
{
    public class BlazoradeAuthenticationOptions
    {

        /// <summary>
        /// The full URI to the metadata JSON document for the authorization endpoint to use.
        /// </summary>
        public string? MetadataUri { get; set; }

        /// <summary>
        /// Defines what type of storage is used when caching tokens.
        /// </summary>
        /// <remarks>
        /// The default cache mode is <see cref="TokenCacheMode.Session"/>.
        /// </remarks>
        public TokenCacheMode CacheMode { get; set; } = TokenCacheMode.Session;

    }
}
