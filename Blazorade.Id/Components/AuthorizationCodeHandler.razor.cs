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
        public SerializationService SerializationService { get; set; } = null!;

        [Inject]
        public NavigationManager NavMan { get; set; } = null!;

        [Inject]
        public IHostEnvironmentAuthenticationStateProvider AuthStateSetter { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var uri = new Uri(this.NavMan.Uri);

            var parameters = new Dictionary<string, StringValues>();
            var queryParameters = new Dictionary<string, StringValues>();
            if(uri.Fragment?.Length > 1)
            {
                parameters = QueryHelpers.ParseQuery(uri.Fragment.Substring(1));
            }
            else if(uri.Query?.Length > 1)
            {
                queryParameters = QueryHelpers.ParseQuery(uri.Query.Substring(1));
            }

            foreach(var key in queryParameters.Keys)
            {
                if(!parameters.ContainsKey(key))
                {
                    parameters[key] = queryParameters[key];
                }
            }

            if(parameters.ContainsKey("code") || parameters.ContainsKey("token") || parameters.ContainsKey("id_token"))
            {
                var state = this.SerializationService.DeserializeBase64String<LoginState>(parameters.GetValueOrDefault("state").ToString()) ?? new LoginState();
                parameters.Remove("state");


                string? code = parameters.GetValueOrDefault("code"),
                    accessToken = parameters.GetValueOrDefault("token"),
                    idToken = parameters.GetValueOrDefault("id_token");

                LoginCompletedState? loginState = null;

                if (code?.Length > 0) loginState = await this.BidService.CompleteLoginAsync(code, state);


                if(null != loginState)
                {
                    var encoded = this.SerializationService.SerializeToBase64String(loginState);
                    parameters.Add("state", encoded);
                }

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
