using Blazorade.Id.Core.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The Blazorade Id Service is the main service that you use in your Blazor application
    /// to access identity and access tokens for your application users.
    /// </summary>
    public class BlazoradeIdService
    {
        /// <summary>
        /// Creates a new instance of the service class with dependencies.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The exception that is thrown if any of the parameters are <c>null</c>.
        /// </exception>
        public BlazoradeIdService(TokenService tokenService, EndpointService endpointService, INavigator navigator, IOptions<AuthorityOptions> authOptions)
        {
            this.Token = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.Endpoint = endpointService ?? throw new ArgumentNullException(nameof(endpointService));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.AuthOptions = authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));
        }

        private readonly TokenService Token;
        private readonly EndpointService Endpoint;
        private readonly INavigator Navigator;
        private readonly AuthorityOptions AuthOptions;


        /// <summary>
        /// Returns the ID token for the currently logged in user.
        /// </summary>
        /// <remarks>
        /// If a valid token cannot be acquired, an authentication process will be started in order to authenticate
        /// the user and get the identity token.
        /// </remarks>
        public async ValueTask<JwtSecurityToken?> GetIdentityTokenAsync()
        {
            var idToken = await this.Token.GetIdentityTokenAsync();
            if(null == idToken)
            {
                var builder = await this.Endpoint.CreateAuthorizationUriBuilderAsync();
                var authUri = builder
                    .WithRedirectUri(this.Navigator.CurrentUri)
                    .WithScope(this.AuthOptions.Scope)
                    .Build();

                await this.Navigator.NavigateToAsync(authUri);
            }

            return idToken;
        }
    }
}
