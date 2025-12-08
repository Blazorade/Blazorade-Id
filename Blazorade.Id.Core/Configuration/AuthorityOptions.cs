using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Configuration
{
    /// <summary>
    /// Defines options that define how to connect to a specific authority (identity provider).
    /// </summary>
    public class AuthorityOptions
    {

        /// <summary>
        /// The Client ID (Application ID) of the application that uses these options.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// The full URI to the metadata JSON document for the authorization endpoint to use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The metadata document contains all endpoints that Blazorade ID need to work.
        /// </para>
        /// <para>
        /// If the metadata URI is not specified, you need to separately specify both
        /// <see cref="AuthorizationEndpoint"/> and <see cref="TokenEndpoint"/>. If you
        /// want to support signing out, you also need to configure <see cref="EndSessionEndpoint"/>.
        /// </para>
        /// <para>
        /// If you specify the metadata URI, you can still "override" <see cref="AuthorizationEndpoint"/>,
        /// <see cref="TokenEndpoint"/> or <see cref="EndSessionEndpoint"/> by specifying them
        /// separately.
        /// </para>
        /// </remarks>
        public string? MetadataUri { get; set; }

        /// <summary>
        /// The authorization endpoint of your selected identity provider.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The authorization endpoint for Microsoft Entra ID (formerly Azure AD) tenants
        /// typically looks something like this.
        /// </para>
        /// <para>
        /// https://login.microsoftonline.com/[TenantId]/oauth2/v2.0/authorize
        /// </para>
        /// <para>
        /// In Microsoft Entra External ID, authorization endpoints usually look something like
        /// the following.
        /// </para>
        /// <para>
        /// https://[TenantName].ciamlogin.com/[TenantName].onmicrosoft.com/oauth2/v2.0/authorize
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
        /// The token endpoint for Microsoft Entra ID tenants typically looks like this.
        /// </para>
        /// <para>
        /// https://login.microsoftonline.com/[TenantId]/oauth2/v2.0/token
        /// </para>
        /// <para>
        /// For Microsoft Entra External ID, the token endpoint is typically constructed as follows.
        /// </para>
        /// <para>
        /// https://[TenantName].ciamlogin.com/[TenantName].onmicrosoft.com/oauth2/v2.0/token
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
        /// The endpoint that should be called when the user wants to log out.
        /// </summary>
        public string? EndSessionEndpoint { get; set; }

        /// <summary>
        /// The redirect URI where to redirect the users back after logging in.
        /// </summary>
        /// <remarks>
        /// This must be configured in the application registration and is typically
        /// stored as a URI relative to the home / root URI of the application.
        /// </remarks>
        public string? RedirectUri { get; set; }

        /// <summary>
        /// Defines the default scopes to use when acquiring tokens from this authority.
        /// </summary>
        public const string DefaultScope = "openid profile email";

        /// <summary>
        /// The scope to use by default when acquiring tokens from this authority. Multiple scopes are separated by a space.
        /// </summary>
        /// <remarks>
        /// The default is <c>openid profile email</c>.
        /// </remarks>
        public string? Scope { get; set; } = DefaultScope;
    }
}
