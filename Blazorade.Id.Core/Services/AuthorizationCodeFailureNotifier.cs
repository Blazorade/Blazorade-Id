using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The default authorization code failure handler.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class AuthorizationCodeFailureNotifier : IAuthorizationCodeFailureNotifier
    {
        private EventHandler<AuthorizationCodeResult>? FailedHandler;
        /// <inheritdoc/>
        public event EventHandler<AuthorizationCodeResult> Failed
        {
            add => this.FailedHandler += value;
            remove => this.FailedHandler -= value;
        }

        /// <inheritdoc/>
        public Task HandleFailureAsync(AuthorizationCodeResult failureResult)
        {
            this.OnFailed(failureResult);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Fires the <see cref="Failed"/> event.
        /// </summary>
        protected virtual void OnFailed(AuthorizationCodeResult e)
        {
            this.FailedHandler?.Invoke(this, e);
        }
    }
}
