using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Defines the interface for a service that handles user authentication.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Creates a claims principal from the identity token for the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// If an identity token is not available and cannot be acquired, this method returns <see langword="null"/>.
        /// </remarks>
        /// <param name="options">Options for the sign-in process.</param>
        Task<ClaimsPrincipal?> SignInAsync(SignInOptions? options = null);

        /// <summary>
        /// Signs the user out.
        /// </summary>
        /// <param name="options">Options for the sign-out process.</param>
        Task SignOutAsync(SignOutOptions? options = null);
    }
}
