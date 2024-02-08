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
    public class BlazorPersistentStorage : StorageBase, IPersistentStorage
    {
        /// <inheritdoc/>
        public BlazorPersistentStorage(ILocalStorageService service)
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
        public override ValueTask<T> GetItemAsync<T>(string key)
        {
            return this.Service.GetItemAsync<T>(key);
        }

        /// <inheritdoc/>
        public override ValueTask RemoveItemAsync(string key)
        {
            return this.Service.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public override ValueTask SetItemAsync<T>(string key, T value)
        {
            return this.Service.SetItemAsync(key, value);
        }
    }
}
