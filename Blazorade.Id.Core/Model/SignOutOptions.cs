using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    /// <summary>
    /// Options for signing users out.
    /// </summary>
    public class SignOutOptions
    {
        /// <summary>
        /// The URI to redirect the user to after sign-out is complete.
        /// </summary>
        /// <remarks>
        /// This URI is sent to the end session endpoint at the configured identity provider.
        /// An identity provider may require that this URI is pre-registered in order for the 
        /// redirection to work.
        /// </remarks>
        public string? RedirectUri { get; set; }

        /// <summary>
        /// When you set this to <see langword="true"/>, the sign-out request to the identity 
        /// provider's end session endpoint is skipped. Only token stores are cleared.
        /// </summary>
        public bool SkipEndIdpSession { get; set; }

        /// <summary>
        /// If set to <see langword="true"/>, the base URI of the application is used as the redirect URI.
        /// </summary>
        public bool UseDefaultRedirectUri { get; set; }
    }
}
