using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    partial class TokenHandler
    {

        [Parameter]
        public string? OptionsKey { get; set; }

        [Parameter]
        public bool IsLogout { get; set; }

        [Parameter]
        public string? PostLogoutRedirectUri { get; set; }



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


        private AuthenticationOptions GetOptions()
        {
            var options = this.AuthOptions.Create(this.OptionsKey ?? string.Empty);
            return options;
        }
    }
}
