using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    /// <summary>
    /// The authentication state provider configured for Blazorade ID.
    /// </summary>
    /// <remarks>
    /// The responsibility of this service is to notify ASP.NET Core of changes
    /// in the authentication status of a user, so that you can then use components
    /// like <see cref="AuthorizeView"/>.
    /// </remarks>
    public class BlazoradeIdAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        /// <summary>
        /// Creates an instance of the provider.
        /// </summary>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return this.AuthStateProvider;
        }

        private Task<AuthenticationState> AuthStateProvider = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        void IHostEnvironmentAuthenticationStateProvider.SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            this.AuthStateProvider = authenticationStateTask;
            this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        }
    }
}
