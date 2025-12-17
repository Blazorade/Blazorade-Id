using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A property store implementation that stores properties in memory.
    /// </summary>
    public class InMemoryPropertyStore : PropertyStoreBase
    {
        private Dictionary<string, object> Properties = new Dictionary<string, object>();

        /// <inheritdoc/>
        public override ValueTask<bool> ContainsKeyAsync(string key)
        {
            return ValueTask.FromResult<bool>(this.Properties.ContainsKey(key));
        }

        /// <inheritdoc/>
        public async override ValueTask<T> GetPropertyAsync<T>(string key)
        {
            if (await this.ContainsKeyAsync(key))
            {
                return (T)this.Properties[key];
            }
            return default!;
        }

        /// <inheritdoc/>
        public override ValueTask RemovePropertyAsync(string key)
        {
            this.Properties.Remove(key);
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public override ValueTask SetPropertyAsync<T>(string key, T value)
        {
            this.Properties[key] = value!;
            return ValueTask.CompletedTask;
        }
    }
}
