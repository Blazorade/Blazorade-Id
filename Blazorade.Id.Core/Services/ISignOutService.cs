using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The service interface for services that handle user sign-out.
    /// </summary>
    /// <remarks>
    /// A sign-out service is responsible for signing the user out of the application. This means clearing
    /// out all cached tokens and user information, as well as redirecting the user to the identity provider's
    /// end session endpoint.
    /// </remarks>
    public interface ISignOutService
    {
        /// <summary>
        /// Signs the user out.
        /// </summary>
        /// <param name="options">Options for the sign-out process.</param>
        Task SignOutAsync(SignOutOptions? options = null);
    }
}
