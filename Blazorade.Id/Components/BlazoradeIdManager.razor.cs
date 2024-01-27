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
        [Inject]
        public BlazoradeIdService IdService { get;set; }

        [Parameter]
        public EventCallback<object> IdentityTokenAcquired { get; set; }

        [Parameter]
        public string? OptionsKey { get; set; }

        [Parameter]
        public bool IsLogout { get; set; }

        [Parameter]
        public string? PostLogoutRedirectUri { get; set; }


        public async ValueTask LogoutAsync(LogoutOptions? options = null)
        {
            await this.IdService.LogoutAsync(options: options);
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if(this.IsLogout)
            {
                await this.TokenService.LogoutAsync(new LogoutOptions
                {
                    Key = this.OptionsKey,
                    PostLogoutRedirectUri = this.PostLogoutRedirectUri
                });
            }

            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
