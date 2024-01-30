using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    partial class AuthorizationCodeHandler
    {

        [Inject]
        public BlazoradeIdService BidService { get; set; } = null!;

        [Inject]
        public NavigationManager NavMan { get; set; } = null!;

        [Inject]
        public IHostEnvironmentAuthenticationStateProvider AuthStateSetter { get; set; } = null!;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (this.NavMan.Uri.Contains("#code=") && this.NavMan.Uri.Contains("&state="))
            {
                var uri = new Uri(this.NavMan.Uri);
                var parameters = QueryHelpers.ParseQuery(uri.Fragment.Substring(1));
                var code = parameters.GetValueOrDefault("code").ToString();
                var state = this.BidService.DeserializeState<LoginState>(parameters.GetValueOrDefault("state").ToString());

                parameters.Remove("code");
                parameters.Remove("state");

                LoginCompletedState loginState = await this.BidService.CompleteLoginAsync(code, state);
                var encoded = this.BidService.SerializeState(loginState);
                parameters.Add("state", encoded);

                var redirUri = state?.Uri ?? this.NavMan.BaseUri ?? "/";
                if (redirUri.Contains('#')) redirUri = redirUri.Substring(0, redirUri.IndexOf('#'));
                redirUri = QueryHelpers.AddQueryString(redirUri, parameters).Replace('?', '#');
                this.NavMan.NavigateTo(redirUri);
            }

            var username = await this.BidService.GetCurrentUsernameAsync();
            if (username?.Length > 0)
            {
                var user = await this.BidService.GetCurrentUserPrincipalAsync() ?? new ClaimsPrincipal();
                this.AuthStateSetter.SetAuthenticationState(Task.FromResult(new AuthenticationState(user)));
            }
        }

    }
}
