using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A property store implementation that stores properties in the browser's local storage.
    /// </summary>
    public class BrowserLocalStoragePropertyStore : IPropertyStore
    {
        /// <inheritdoc/>
        public BrowserLocalStoragePropertyStore(ILocalStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ILocalStorageService Service;

        /// <inheritdoc/>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public async Task<T> GetPropertyAsync<T>(string key)
        {
            return await this.Service.GetItemAsync<T>(key) ?? default!;
        }

        /// <inheritdoc/>
        public async Task RemovePropertyAsync(string key)
        {
            await this.Service.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public async Task SetPropertyAsync<T>(string key, T value)
        {
            await this.Service.SetItemAsync(key, value);
        }
    }
}
