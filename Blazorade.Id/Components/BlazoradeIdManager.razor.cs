using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    partial class BlazoradeIdManager
    {
        /// <summary>
        /// The <see cref="BlazoradeIdService"/> configured for the application.
        /// </summary>
        /// <remarks>
        /// This is automatically injected to the component during initialization.
        /// </remarks>
        [Inject]
        public BlazoradeIdService IdService { get; set; } = null!;

        /// <summary>
        /// Triggered when 
        /// </summary>
        public EventCallback<TokenSet> OnTokensAvailable { get; set; }

        /// <summary>
        /// The key that is used to resolve the configured authority to use when interacting with
        /// this component.
        /// </summary>
        /// <remarks>
        /// This is the same key that you used when registering an authority during startup of
        /// the application using one of the <see cref="BlazoradeIdBuilder.AddAuthority"/> methods.
        /// </remarks>
        [Parameter]
        public string? AuthorityKey { get; set; }




        /// <summary>
        /// Logs the current user out and clears all cached tokens.
        /// </summary>
        /// <param name="postLogoutRedirectUri">An optional URI to redirect the user to.</param>
        /// <param name="redirectToCurrentUri">
        /// Specifies whether to redirect the user back to the current URI. This parameter is only
        /// used if no URI is specified in <paramref name="postLogoutRedirectUri"/>.
        /// </param>
        /// <returns></returns>
        public async ValueTask LogoutAsync(string? postLogoutRedirectUri = null, bool redirectToCurrentUri = true)
        {
            await this.IdService.LogoutAsync(postLogoutRedirectUri: postLogoutRedirectUri, redirectToCurrentUri: redirectToCurrentUri);
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {

            }

            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
