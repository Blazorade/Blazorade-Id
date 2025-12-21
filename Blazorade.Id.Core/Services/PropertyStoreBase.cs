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
    public abstract class PropertyStoreBase : IPropertyStore
    {

        /// <inheritdoc/>
        public abstract Task<bool> ContainsKeyAsync(string key);

        /// <inheritdoc/>
        public abstract Task<T> GetPropertyAsync<T>(string key);


        /// <inheritdoc/>
        public abstract Task RemovePropertyAsync(string key);

        /// <inheritdoc/>
        public virtual async Task RemovePropertiesAsync(IEnumerable<string> keys)
        {
            foreach(var key in keys ?? Enumerable.Empty<string>())
            {
                await this.RemovePropertyAsync(key);
            }
        }

        /// <inheritdoc/>
        public abstract Task SetPropertyAsync<T>(string key, T value);

    }
}
