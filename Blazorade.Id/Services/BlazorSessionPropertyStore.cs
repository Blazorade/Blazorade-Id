using Blazorade.Id.Core.Services;
using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A property storage implementation that uses Blazored.SessionStorage as the underlying storage mechanism.
    /// </summary>
    public class BlazorSessionPropertyStore : PropertyStoreBase
    {
        /// <inheritdoc/>
        public BlazorSessionPropertyStore(ISessionStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ISessionStorageService Service;

        /// <inheritdoc/>
        public override ValueTask<bool> ContainsKeyAsync(string key)
        {
            return this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public override ValueTask<T> GetPropertyAsync<T>(string key)
        {
            return this.Service.GetItemAsync<T>(key);
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
