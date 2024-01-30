using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    public class BlazoradeIdAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return this.StateTask;
        }

        private Task<AuthenticationState> StateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        void IHostEnvironmentAuthenticationStateProvider.SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            this.StateTask = authenticationStateTask;
            this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        }
    }
}
