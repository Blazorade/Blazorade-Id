using Blazorade.Id.Core.Services;
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
        public override ValueTask<bool> ContainsKeyAsync(string key)
        {
            return this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public async override ValueTask<T> GetPropertyAsync<T>(string key)
        {
            return await this.Service.GetItemAsync<T>(key) ?? default!;
        }

        /// <inheritdoc/>
        public override ValueTask RemovePropertyAsync(string key)
        {
            return this.Service.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public override ValueTask SetPropertyAsync<T>(string key, T value)
        {
            return this.Service.SetItemAsync(key, value);
        }
    }
}
