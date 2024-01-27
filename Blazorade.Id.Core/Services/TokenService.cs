using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class TokenService
    {
        public TokenService(IOptionsFactory<AuthenticationOptions> optionsFactory, IHttpClientFactory clientFactory, EndpointService epService, ISessionStorage sessionStorage, IPersistentStorage persistentStorage, INavigator navigator)
        {
            this.OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.EPService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
            this.PersistentStorage = persistentStorage ?? throw new ArgumentNullException(nameof(persistentStorage));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        private readonly IOptionsFactory<AuthenticationOptions> OptionsFactory;
        private readonly IHttpClientFactory ClientFactory;
        private readonly EndpointService EPService;
        private readonly ISessionStorage SessionStorage;
        private readonly IPersistentStorage PersistentStorage;
        private readonly INavigator Navigator;

        public async ValueTask<string?> AcquireIdentityTokenAsync(TokenRequestOptions options)
        {

            return null;
        }

        public async ValueTask LogoutAsync(LogoutOptions? options = null)
        {
            var authOptions = this.GetAuthOptions(options?.Key);
            var builder = await this.EPService.CreateEndSessionUriBuilderAsync(authOptions);
            var logoutUri = builder
                .WithPostLogoutRedirectUri(options?.PostLogoutRedirectUri)
                .Build();

            await this.Navigator.NavigateToAsync(logoutUri);
        }

        public async ValueTask<TokenResponse> RedeemAuthorizationCodeAsync(TokenRequestOptions options)
        {
            var authOptions = this.GetAuthOptions(options?.Key);
            var code = options?.AuthorizationToken ?? throw new ArgumentException("No authorization code specified on the options.", nameof(options));

            return new TokenResponse(new TokenError());
        }



        private AuthenticationOptions GetAuthOptions(string? key)
        {
            var authOptions = this.OptionsFactory.Create(key ?? string.Empty);
            if (null == authOptions)
            {
                throw new ArgumentException("No authentication options found with the key specified in key.", nameof(key));
            }
            return authOptions;
        }
    }
}
