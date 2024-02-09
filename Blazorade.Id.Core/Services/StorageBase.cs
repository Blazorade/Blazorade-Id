using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A base class for storage implementations.
    /// </summary>
    public abstract class StorageBase : IStorage
    {

        /// <inheritdoc/>
        public abstract ValueTask<bool> ContainsKeyAsync(string key);

        /// <inheritdoc/>
        public abstract ValueTask<T> GetItemAsync<T>(string key);

        /// <inheritdoc/>
        public abstract ValueTask RemoveItemAsync(string key);

        /// <inheritdoc/>
        public virtual async ValueTask RemoveItemsAsync(IEnumerable<string> keys)
        {
            foreach(var key in keys ?? Enumerable.Empty<string>())
            {
                await this.RemoveItemAsync(key);
            }
        }

        /// <inheritdoc/>
        public abstract ValueTask SetItemAsync<T>(string key, T value);

    }
}
