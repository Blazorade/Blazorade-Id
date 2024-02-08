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
        public BlazoradeIdAuthenticationStateProvider(StorageFacade storage)
        {
            this.StorageFacade = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        private readonly StorageFacade StorageFacade;

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = await this.AuthStateProvider;
            if(state?.User?.Identity?.IsAuthenticated == false)
            {
                ClaimsPrincipal? user = null;
                try
                {
                    user = await this.LoadCurrentPrincipalAsync();
                    var foo = null != user;
                }
                catch { }
                
                if(user?.Identity?.IsAuthenticated == true)
                {
                    state = new AuthenticationState(user);
                }
            }

            return state;
        }

        private Task<AuthenticationState> AuthStateProvider = Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        void IHostEnvironmentAuthenticationStateProvider.SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            this.AuthStateProvider = authenticationStateTask;
            this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        }

        private async ValueTask<ClaimsPrincipal?> LoadCurrentPrincipalAsync()
        {
            var idToken = await this.StorageFacade.GetIdentityTokenAsync();
            if (null != idToken)
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity(idToken.Claims, "oidc", "name", "roles"));
                return principal;
            }

            return null;
        }
    }
}
