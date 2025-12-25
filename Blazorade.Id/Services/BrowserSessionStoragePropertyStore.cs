using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A property store implementation that stores properties in the browser's session storage.
    /// </summary>
    public class BrowserSessionStoragePropertyStore : IPropertyStore
    {
        /// <inheritdoc/>
        public BrowserSessionStoragePropertyStore(ISessionStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ISessionStorageService Service;

        /// <inheritdoc/>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public async Task<T> GetPropertyAsync<T>(string key)
        {
            return await this.Service.GetItemAsync<T>(key);
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
