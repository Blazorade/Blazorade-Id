using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorade.Id.Core.Services;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents the results of an authorization code request from an implementation of
    /// the <see cref="IAuthorizationCodeProvider"/> service interface.
    /// </summary>
    public class AuthorizationCodeResult
    {
        /// <summary>
        /// The authorization code if available.
        /// </summary>
        /// <remarks>
        /// If the authorization code request failed, this property must be <see langword="null"/>.
        /// The reason for the failure can be found in the <see cref="FailureReason"/> property.
        /// </remarks>
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the reason for the authorization code failure, if any.
        /// </summary>
        /// <remarks>
        /// If <see cref="Code"/> is not <see langword="null"/>, this property must be <see langword="null"/>.
        /// </remarks>
        public AuthorizationCodeFailureReason? FailureReason { get; set; }
    }
}
