using Blazorade.Id.Core.Services;
using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <inheritdoc/>
    public class BlazorSessionStorage : StorageBase, ISessionStorage
    {
        /// <inheritdoc/>
        public BlazorSessionStorage(ISessionStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ISessionStorageService Service;

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
        public override ValueTask<T> GetItemAsync<T>(string key)
        {
            return this.Service.GetItemAsync<T>(key);
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
        public override ValueTask SetItemAsync<T>(string key, T value)
        {
            return this.Service.SetItemAsync(key, value);
        }
    }
}
