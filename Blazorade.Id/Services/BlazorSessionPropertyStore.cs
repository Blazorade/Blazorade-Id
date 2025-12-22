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
        public override async Task<bool> ContainsKeyAsync(string key)
        {
            return await this.Service.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public override async Task<T> GetPropertyAsync<T>(string key)
        {
            return await this.Service.GetItemAsync<T>(key);
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
