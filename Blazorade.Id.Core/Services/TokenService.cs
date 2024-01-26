using Blazorade.Id.Core.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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

        public string? AcquireIdentityTokenAsync()
        {

            return null;
        }

        public string? AcquireIdentityTokenAsync(string key)
        {

            return null;
        }
    }
}
