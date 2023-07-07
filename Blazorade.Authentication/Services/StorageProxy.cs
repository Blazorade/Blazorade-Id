using Blazorade.Authentication.Configuration;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Authentication.Services
{
    public class StorageProxy
    {
        public StorageProxy(IOptionsFactory<BlazoradeAuthenticationOptions> factory, ILocalStorageService localStorage, ISessionStorageService sessionStorage)
        {
            this.OptionsFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.LocalStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        internal const string IdentityToken = nameof(IdentityToken);
        internal const string AccessToken = nameof(AccessToken);
        internal const string RefreshToken = nameof(RefreshToken);

        private readonly IOptionsFactory<BlazoradeAuthenticationOptions> OptionsFactory;
        private readonly ILocalStorageService LocalStorage;
        private readonly ISessionStorageService SessionStorage;

        public async Task<T> GetItemAsync<T>(string key)
        {
            T item = default!;
            var options = this.OptionsFactory.Create("");

            if(options?.CacheMode == TokenCacheMode.Persistent)
            {
                item = await this.LocalStorage.GetItemAsync<T>(key);
            }
            else if(options?.CacheMode == TokenCacheMode.Session)
            {
                item = await this.SessionStorage.GetItemAsync<T>(key);
            }

            return item;
        }

    }
}
