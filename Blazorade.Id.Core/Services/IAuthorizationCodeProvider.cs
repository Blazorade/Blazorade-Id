using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Defines the interface for services that can provide authorization codes.
    /// </summary>
    /// <remarks>
    /// An authorization code provider is responsible for sending the user to the
    /// authorization endpoint of the identity provider and returning the authorization code
    /// </remarks>
    public interface IAuthorizationCodeProvider
    {
        /// <summary>
        /// Asynchronously initiates an authorization request and retrieves an authorization 
        /// code using the specified options.
        /// </summary>
        Task<AuthorizationCodeResult> GetAuthorizationCodeAsync(GetTokenOptions options);
    }
}
