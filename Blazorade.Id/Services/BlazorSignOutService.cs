using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Sign-out service for Blazor applications.
    /// </summary>
    public class BlazorSignOutService : ISignOutService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlazorSignOutService"/> class.
        /// </summary>
        public BlazorSignOutService(ITokenStore tokenStore, IAuthenticationStateNotifier authStateNotifier, NavigationManager navMan)
        {
            this.TokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
            this.AuthStateNotifier = authStateNotifier ?? throw new ArgumentNullException(nameof(authStateNotifier));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
        }

        private readonly ITokenStore TokenStore;
        private readonly IAuthenticationStateNotifier AuthStateNotifier;
        private readonly NavigationManager NavMan;

        /// <inheritdoc/>
        public async Task SignOutAsync(SignOutOptions? options = null)
        {
            options = options ?? new SignOutOptions();
            options.RedirectUri = options.RedirectUri ?? (options.UseDefaultRedirectUri ? this.NavMan.Uri : null);

            await this.TokenStore.ClearAllAsync();
            await this.AuthStateNotifier.StateHasChangedAsync();


        }
    }
}
