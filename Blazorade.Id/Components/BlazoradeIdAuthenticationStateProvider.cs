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
    public class BlazoradeIdAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
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
