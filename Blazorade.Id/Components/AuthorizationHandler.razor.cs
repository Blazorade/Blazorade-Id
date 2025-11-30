using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Components
{
    partial class AuthorizationHandler
    {

        /// <summary>
        /// Indicates whether the authorization handler is processing sign-in information.
        /// </summary>
        [Parameter]
        public bool Processing { get; set; }



        [Inject]
        TokenService TokenService { get; set; } = null!;

        [Inject]
        SerializationService SerializationService { get; set; } = null!;

        [Inject]
        NavigationManager NavMan { get; set; } = null!;

        [Inject]
        IHostEnvironmentAuthenticationStateProvider AuthStateSetter { get; set; } = null!;



        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                await this.HandleCurrentAuthenticationAsync();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            try
            {
                if (this.NavMan.Uri.Contains('#') || this.NavMan.Uri.Contains('?'))
                {
                    await this.ProcessUriAsync(this.NavMan.Uri);
                }
                else
                {
                    await this.HandleCurrentAuthenticationAsync();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }



        private async ValueTask ProcessUriAsync(string url)
        {
            var uri = new Uri(this.NavMan.Uri);

            var parameters = new Dictionary<string, StringValues>();
            var queryParameters = new Dictionary<string, StringValues>();
            if (uri.Fragment?.Length > 1)
            {
                parameters = QueryHelpers.ParseQuery(uri.Fragment.Substring(1));
            }
            else if (uri.Query?.Length > 1)
            {
                queryParameters = QueryHelpers.ParseQuery(uri.Query.Substring(1));
            }

            foreach (var key in queryParameters.Keys)
            {
                if (!parameters.ContainsKey(key))
                {
                    parameters[key] = queryParameters[key];
                }
            }

            if (parameters.ContainsKey("code") || parameters.ContainsKey("token") || parameters.ContainsKey("id_token"))
            {
                this.Processing = true;
                try
                {
                    await this.ProcessParametersAsync(parameters);
                }
                finally
                {
                    this.Processing = false;
                }
            }
            else
            {
                await this.HandleCurrentAuthenticationAsync();
            }

        }

        private async ValueTask HandleCurrentAuthenticationAsync()
        {
            JwtSecurityToken? idToken = null;
            try
            {
                idToken = await this.TokenService.GetIdentityTokenAsync();
            }
            catch { }

            if(null != idToken)
            {
                var principal = this.CreatePrincipal(idToken);
                if(null != principal)
                {
                    this.AuthStateSetter.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
                }
            }
        }

        private async ValueTask ProcessParametersAsync(Dictionary<string, StringValues> parameters)
        {
            LoginState state = null!;

            try
            {
                state = parameters.ContainsKey("state")
                    ? this.SerializationService.DeserializeBase64String<LoginState>(parameters.GetValueOrDefault("state").ToString()) ?? new LoginState()
                    : new LoginState();
            }
            catch
            {
                // In case of an exception, set the state to an empty state.
                state = new LoginState();
            }

            parameters.Remove("state");
            parameters.Remove("session_state");

            string? code = parameters.GetValueOrDefault("code"),
                accessToken = parameters.GetValueOrDefault("token"),
                idToken = parameters.GetValueOrDefault("id_token"),
                nonce = parameters.GetValueOrDefault("nonce");

            parameters.Remove("code");
            parameters.Remove("id_token");
            parameters.Remove("token");

            OperationResult<TokenSet>? codeResult = null!;
            if (code?.Length > 0)
            {
                // If an authorization code is specified, then we use that to get both an
                // identity token and an access token, and ignore any tokens that
                // were sent in the URL.
                codeResult = await this.TokenService.ProcessAuthorizationCodeAsync(code, this.NavMan.BaseUri, nonce);
                this.NotifyAuthenticationStateChanged(codeResult?.Value?.GetIdentityToken());
            }

            if (!(codeResult?.Value?.IdentityToken?.Length > 0) && idToken?.Length > 0)
            {
                var token = await this.TokenService.ProcessIdentityTokenAsync(idToken, nonce);
                this.NotifyAuthenticationStateChanged(token?.Value?.ParseToken());
            }

            if (!(codeResult?.Value?.AccessToken?.Length > 0) && accessToken?.Length > 0)
            {
                await this.TokenService.ProcessAccessTokenAsync(accessToken);
            }

            var redirUri = state?.Uri ?? this.NavMan.BaseUri ?? "/";
            this.NavMan.NavigateTo(redirUri);
        }

        private void NotifyAuthenticationStateChanged(JwtSecurityToken? idToken)
        {
            if(null != idToken)
            {
                var principal = this.CreatePrincipal(idToken);
                this.AuthStateSetter.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
            }
            else
            {
                this.AuthStateSetter.SetAuthenticationState(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
            }
        }

        private ClaimsPrincipal? CreatePrincipal(JwtSecurityToken? idToken)
        {
            if(null != idToken)
            {
                return new ClaimsPrincipal(new ClaimsIdentity(idToken.Claims, "oidc", "name", "roles"));
            }

            return null;
        }

    }
}
