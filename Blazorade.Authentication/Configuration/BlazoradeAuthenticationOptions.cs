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
        /// The authorization endpoint of your selected identity provider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The authorization endpoint for Azure AD tenants typically looks something like this.
        /// </para>
        /// <para>
        /// https://login.microsoftonline.com/[TenantId]/oauth2/v2.0/authorize
        /// </para>
        /// <para>
        /// For Azure AD B2C tenants the authorization endpoint looks typically like this.
        /// </para>
        /// <para>
        /// https://[TenantName].b2clogin.com/[TenantName].onmicrosoft.com/oauth2/v2.0/authorize?p=[PolicyId]
        /// </para>
        /// </remarks>
        public string? AuthorizationEndpoint { get; set; }

        /// <summary>
        /// The token endpoint of your selected identity provider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The token endpoint for Azure AD tenants typically looks like this.
        /// </para>
        /// <para>
        /// https://login.microsoftonline.com/[TenantId]/oauth2/v2.0/token
        /// </para>
        /// <para>
        /// For Azure AD B2C tenants the token endpoint looks typically something like this.
        /// </para>
        /// <para>
        /// https://[TenantName].b2clogin.com/[TenantName].onmicrosoft.com/oauth2/v2.0/token?p=[PolicyId]
        /// </para>
        /// </remarks>
        public string? TokenEndpoint { get; set; }

        /// <summary>
        /// Defines what type of storage is used when caching tokens.
        /// </summary>
        /// <remarks>
        /// The default cache mode is <see cref="TokenCacheMode.Session"/>.
        /// </remarks>
        public TokenCacheMode CacheMode { get; set; } = TokenCacheMode.Session;

    }
}
