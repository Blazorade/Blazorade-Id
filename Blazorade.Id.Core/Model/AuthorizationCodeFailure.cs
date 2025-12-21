using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Defines the possible failure reasons for an authorization code request.
    /// </summary>
    public enum AuthorizationCodeFailure
    {
        /// <summary>
        /// The authorization code request was blocked. This could be due to popup blockers or similar.
        /// </summary>
        Blocked = 1,
        /// <summary>
        /// The authorization code request was cancelled by the user.
        /// </summary>
        CancelledByUser = 2,
        /// <summary>
        /// The authorization code request timed out.
        /// </summary>
        TimedOut = 3,
        /// <summary>
        /// The authorization code request failed due to a system failure.
        /// </summary>
        SystemFailure = 4,
    }
}
