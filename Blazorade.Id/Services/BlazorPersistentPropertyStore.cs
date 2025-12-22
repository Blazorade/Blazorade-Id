using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <inheritdoc/>
    public class BlazorPersistentPropertyStore : PropertyStoreBase
    {
        /// <inheritdoc/>
        public BlazorPersistentPropertyStore(ILocalStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ILocalStorageService Service;

        /// <inheritdoc/>
        public override async Task<bool> ContainsKeyAsync(string key)
        {
            return await this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public async override Task<T> GetPropertyAsync<T>(string key)
        {
            return await this.Service.GetItemAsync<T>(key) ?? default!;
        }

        /// <inheritdoc/>
        public override async Task RemovePropertyAsync(string key)
        {
            await this.Service.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public override async Task SetPropertyAsync<T>(string key, T value)
        {
            await this.Service.SetItemAsync(key, value);
        }
    }
}
