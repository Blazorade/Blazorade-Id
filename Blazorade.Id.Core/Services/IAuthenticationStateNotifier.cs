using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// The service contract for a service that notifies the application about changes in authentication state.
    /// </summary>
    /// <remarks>
    /// Other services use this service when authentication state has changed so that the application can be notified.
    /// </remarks>
    public interface IAuthenticationStateNotifier
    {

        /// <summary>
        /// Called from other services to notify the application that the authentication state has changed.
        /// </summary>
        Task StateHasChangedAsync();
    }
}
