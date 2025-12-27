using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A base class for property store implementations that use web storage in a browser.
    /// </summary>
    public abstract class WebPropertyStoreBase : WebStoreBase , IPropertyStore
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        protected WebPropertyStoreBase(WebStoreType storeType, IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(storeType, jsRuntime, jsonOptions)
        {
        }

        /// <inheritdoc/>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            var keys = await this.GetKeysAsync();
            return keys.Contains(this.GetKey(key));
        }

        /// <inheritdoc/>
        public async Task<T> GetPropertyAsync<T>(string key)
        {
            var prefixedKey = this.GetKey(key);
            return await this.GetItemAsync<T>(prefixedKey) ?? default!;
        }

        /// <inheritdoc/>
        public async Task RemovePropertyAsync(string key)
        {
            var prefixedKey = this.GetKey(key);
            await this.RemoveItemAsync(prefixedKey);
        }

        /// <inheritdoc/>
        public async Task SetPropertyAsync<T>(string key, T value)
        {
            var prefixedKey = this.GetKey(key);
            await this.SetItemAsync(prefixedKey, value);
        }
    }
}
