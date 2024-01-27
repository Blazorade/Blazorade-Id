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
        public TokenService(IOptionsFactory<AuthenticationOptions> optionsFactory, IHttpClientFactory clientFactory, EndpointService epService)
        {
            this.OptionsFactory = optionsFactory;
            this.ClientFactory = clientFactory;
            this.EPService = epService;
        }

        private readonly IOptionsFactory<AuthenticationOptions> OptionsFactory;
        private readonly IHttpClientFactory ClientFactory;
        private readonly EndpointService EPService;

        public async ValueTask<string?> AcquireIdentityTokenAsync(TokenRequestOptions options)
        {

            return null;
        }

        public async ValueTask<string?> AcquireIdentityTokenAsync(string key, TokenRequestOptions options)
        {

            return null;
        }


        public async ValueTask<TokenResponse> RedeemAuthorizationCodeAsync(string authorizationCode)
        {

            return new TokenResponse(new TokenError());
        }

        public async ValueTask<TokenResponse> RedeemAuthorizationCodeAsync(Uri uri)
        {

            return new TokenResponse(new TokenError());
        }
    }
}
