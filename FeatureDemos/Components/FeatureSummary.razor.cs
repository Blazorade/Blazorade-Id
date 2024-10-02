using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureDemos.Components
{
    /// <summary>
    /// This component demonstrates the most important functionality in Blazorade Id.
    /// </summary>
    partial class FeatureSummary
    {
        [Inject]
        TokenService TokenService { get; set; } = null!;

        [Inject]
        EndpointService EndpointService { get; set; } = null!;

        private IOptions<AuthorityOptions> _AuthorityOptions;
        [Inject]
        IOptions<AuthorityOptions> AuthorityOptions
        {
            get { return _AuthorityOptions; }
            set
            {
                _AuthorityOptions = value;
                this.AuthOptions = _AuthorityOptions?.Value!;
            }
        }

        private AuthorityOptions AuthOptions;

        private string? MetadataUri, AuthEndpoint, TokenEndpoint, EndSessionEndpoint;
        private JwtSecurityToken? IdentityToken, AccessToken;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            this.MetadataUri = this.AuthOptions?.MetadataUri;
            this.AuthEndpoint = await this.EndpointService.GetAuthorizationEndpointAsync();
            this.TokenEndpoint = await this.EndpointService.GetTokenEndpointAsync();
            this.EndSessionEndpoint = await this.EndpointService.GetEndSessionEndpointAsync();
        }

        async Task LogInAsync(MouseEventArgs args)
        {
            this.IdentityToken = await this.TokenService.GetIdentityTokenAsync();
            this.AccessToken = await this.TokenService.GetAccessTokenAsync();
        }
    }
}
