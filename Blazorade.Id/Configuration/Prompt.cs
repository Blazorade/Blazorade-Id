using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Configuration
{
    /// <summary>
    /// Defines different options for prompting users during authentication.
    /// </summary>
    public enum Prompt
    {
        /// <summary>
        /// Attempts to complete the authentication request without prompting the user.
        /// If the request cannot be completed silently, a <c>interaction_required</c>
        /// error code will be returned.
        /// </summary>
        None,

        /// <summary>
        /// Forces the user to log in on every request.
        /// </summary>
        /// <remarks>
        /// This effectively disables single sign-on.
        /// </remarks>
        Login,

        /// <summary>
        /// Triggers the consent dialog to be shown after successful login. Also in
        /// cases where the user already has consented to the requested permissions.
        /// </summary>
        Consent,

        /// <summary>
        /// Provides the user with the option to select an account to log in with.
        /// </summary>
        /// <remarks>
        /// This option effectively disables single sign-on.
        /// </remarks>
        Select_Account
    }
}
