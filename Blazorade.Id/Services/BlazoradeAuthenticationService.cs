using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Model;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    public sealed class BlazoradeAuthenticationService
    {

        public BlazoradeAuthenticationService(IOptionsFactory<BlazoradeAuthenticationOptions> factory, StorageService storage, NavigationManager navMan, IJSRuntime jsRuntime)
        {
            this.OptionsFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
            this.JsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        private readonly IOptionsFactory<BlazoradeAuthenticationOptions> OptionsFactory;
        private readonly StorageService Storage;
        private readonly NavigationManager NavMan;
        private readonly IJSRuntime JsRuntime;

        public Task<TokenResult?> AcquireIdentityTokenAsync()
        {
            return this.AcquireIdentityTokenAsync(string.Empty);
        }

        public async Task<TokenResult?> AcquireIdentityTokenAsync(string optionsName)
        {
            var token = await this.Storage.GetItemAsync<TokenResult>(optionsName, $"{optionsName}.{StorageService.IdentityToken}");
            if(token?.Expires < DateTime.UtcNow)
            {
                token = null;
            }

            if(null == token)
            {
                var options = this.OptionsFactory.Create(optionsName);
                var authBuilder = await EndpointUriBuilder.CreateAuthorizationEndpointUriBuilderAsync(options);
                
            }

            return token;
        }

        public Task<TokenResult?> AcquireAccessTokenAsync()
        {
            return this.AcquireAccessTokenAsync(string.Empty);
        }

        public async Task<TokenResult?> AcquireAccessTokenAsync(string optionsName)
        {
            var token = await this.Storage.GetItemAsync<TokenResult>(optionsName, $"{optionsName}.{StorageService.AccessToken}");

            return token;
        }
    }
}
