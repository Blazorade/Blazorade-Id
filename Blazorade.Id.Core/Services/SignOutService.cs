using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A default implementation of <see cref="ISignOutService"/>.
    /// </summary>
    /// <remarks>
    /// This implementation clears all cached tokens and notifies the application that authentication state
    /// has been changed. Since this implementation has no way of sending a user to the identity provider's
    /// end session endpoint, that part is not implemented in this service.
    /// </remarks>
    public class SignOutService : ISignOutService
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public SignOutService(ITokenStore tokenStore, IAuthenticationStateNotifier authenticationStateNotifier)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.AuthenticationStateNotifier = authenticationStateNotifier ?? throw new ArgumentNullException(nameof(authenticationStateNotifier));
        }

        private readonly ITokenStore TokenStore;
        private readonly IAuthenticationStateNotifier AuthenticationStateNotifier;

        /// <inheritdoc/>
        public async Task SignOutAsync(SignOutOptions? options = null)
        {
            await this.TokenStore.ClearAllAsync();
            await this.AuthenticationStateNotifier.StateHasChangedAsync();
        }
    }
}
