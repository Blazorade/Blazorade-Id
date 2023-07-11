using Blazorade.Authentication.Configuration;
using Blazorade.Authentication.Model;
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

namespace Blazorade.Authentication.Services
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

        public async Task<TokenResult?> AcquireIdentityTokenAsync()
        {
            var token = await this.Storage.GetItemAsync<TokenResult>(StorageService.IdentityToken);

            return token;
        }

        public async Task<TokenResult> AcquireIdentityTokenAsync(string optionsName)
        {
            var token = await this.Storage.GetItemAsync<TokenResult>(optionsName, $"{optionsName}.{StorageService.IdentityToken}");

            return token;
        }

        public async Task<TokenResult?> AcquireAccessTokenAsync()
        {
            var token = await this.Storage.GetItemAsync<TokenResult>(StorageService.AccessToken);

            return token;
        }

        public async Task<TokenResult?> AcquireAccessTokenAsync(string optionsName)
        {
            var token = await this.Storage.GetItemAsync<TokenResult>(optionsName, $"{optionsName}.{StorageService.AccessToken}");

            return token;
        }
    }
}
