using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Authorization
{
    /// <summary>
    /// Defines constants for different authorization errors returned from the authorization server.
    /// These include both standard OAuth2 errors as well as OpenID Connect specific errors.
    /// </summary>
    /// <remarks>
    /// This is not an exhaustive list of all possible errors, but rather a selection of common ones that
    /// may be remedied by retrying authorization request with additional user interaction.
    /// </remarks>
    public static class AuthorizationErrors
    {
        /// <summary>
        /// Represents the error code indicating that user interaction is required to complete the authentication
        /// process.
        /// </summary>
        public const string InteractionRequired = "interaction_required";
        /// <summary>
        /// The authorization server requires end-user authentication.
        /// </summary>
        public const string LoginRequired = "login_required";
        /// <summary>
        /// The end-user is required to select a session at the authorization server.
        /// </summary>
        public const string AccountSelectionRequired = "account_selection_required";
        /// <summary>
        /// The authorization request requires higher privileges than provided by the current access token.
        /// </summary>
        public const string ConsentRequired = "consent_required";
    }
}
