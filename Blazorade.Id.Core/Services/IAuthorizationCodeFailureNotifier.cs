using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A service interface that describes a service that handles authorization code failures.
    /// </summary>
    /// <remarks>
    /// This might be an implementation that displays the error to the user, logs it, or 
    /// performs some other action.
    /// </remarks>
    public interface IAuthorizationCodeFailureNotifier
    {
        /// <summary>
        /// Occurs when an authorization code request fails.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when an attempt to obtain an authorization
        /// code does not succeed. The event provides details about the failure through the <see
        /// cref="AuthorizationCodeResult"/> argument.
        /// </remarks>
        event EventHandler<AuthorizationCodeResult> Failed;

        /// <summary>
        /// When called, handles the specified authorization code failure result.
        /// </summary>
        Task HandleFailureAsync(AuthorizationCodeResult failureResult);
    }
}
