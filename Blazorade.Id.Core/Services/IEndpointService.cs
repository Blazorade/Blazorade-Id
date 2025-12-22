using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Defines the interface for a service that is used to resolve various endpoints.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The main responsibility of an endpoint service is to provide URIs for various
    /// endpoints used during the authentication and authorization process. These URIs
    /// can either be statically configured in the configuration for the application,or
    /// resolved from the OpenID Connect metadata document exposed by the authority,
    /// which is also configured in the configuration for the application.
    /// </para>
    /// <para>
    /// This service interface defines most of the standard endpoints defined by the 
    /// OpenID Connect specification. The methods that return non-nullable strings are
    /// essential to Blazorde Id.
    /// </para>
    /// </remarks>
    public interface IEndpointService
    {

        /// <summary>
        /// Returns the authorization endpoint URI for the configured authority.
        /// </summary>
        Task<string> GetAuthorizationEndpointAsync();

        /// <summary>
        /// Returns the end session endpoint URI for the configured authority.
        /// </summary>
        Task<string?> GetEndSessionEndpointAsync();

        /// <summary>
        /// Returns the token endpoint URI for the configured authority.
        /// </summary>
        Task<string> GetTokenEndpointAsync();

        /// <summary>
        /// Returns the user info endpoint URI for the configured authority.
        /// </summary>
        Task<string?> GetUserInfoEndpointAsync();

        /// <summary>
        /// Returns the device authorization endpoint URI for the configured authority.
        /// </summary>
        Task<string?> GetDeviceAuthorizationEndpointAsync();

        /// <summary>
        /// Returns the Kerberos endpoint URI for the configured authority.
        /// </summary>
        Task<string?> GetKerberosEndpointAsync();
    }
}
