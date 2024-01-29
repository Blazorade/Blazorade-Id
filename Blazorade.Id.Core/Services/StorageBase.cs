using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public abstract class StorageBase : IStorage
    {
        public abstract ValueTask ClearAsync();

        public abstract ValueTask<bool> ContainsKeyAsync(string key);

        public abstract ValueTask<T> GetItemAsync<T>(string key);

        public abstract ValueTask<int> GetItemCountAsync();

        public abstract ValueTask<IEnumerable<string>> GetKeysAsync();

        public abstract ValueTask RemoveItemAsync(string key);

        public async virtual ValueTask RemoveItemsAsync(IEnumerable<string> keys)
        {
            foreach(var key in keys ?? Enumerable.Empty<string>())
            {
                await this.RemoveItemAsync(key);
            }
        }

        public abstract ValueTask SetItemAsync<T>(string key, T value);
    }
}
