using Blazorade.Id.Core.Services;
using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    public class BlazorPersistentStorage : StorageBase, IPersistentStorage
    {
        public BlazorPersistentStorage(ILocalStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ILocalStorageService Service;

        /// <inheritdoc/>
        public override ValueTask ClearAsync()
        {
            return this.Service.ClearAsync();
        }

        /// <inheritdoc/>
        public override ValueTask<bool> ContainsKeyAsync(string key)
        {
            return this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public override ValueTask<string> GetItemAsync(string key)
        {
            return this.Service.GetItemAsStringAsync(key);
        }

        /// <inheritdoc/>
        public override ValueTask<int> GetItemCountAsync()
        {
            return this.Service.LengthAsync();
        }

        /// <inheritdoc/>
        public override ValueTask<IEnumerable<string>> GetKeysAsync()
        {
            return this.Service.KeysAsync();
        }

        /// <inheritdoc/>
        public override ValueTask RemoveItemAsync(string key)
        {
            return this.Service.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public override ValueTask SetItemAsync(string key, string value)
        {
            return this.Service.SetItemAsStringAsync(key, value);
        }
    }
}
