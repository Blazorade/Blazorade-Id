using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A base class for implementations that need to use either local storage or session storage
    /// in a web browser.
    /// </summary>
    public abstract class WebStoreBase : StoreBase
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        protected WebStoreBase(WebStoreType storeType, IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions)
        {
            this.StoragePrefix = storeType == WebStoreType.LocalStorage ? "localStorage" : "sessionStorage";
            this.JsRuntime = jsRuntime;
            if (jsonOptions is null) throw new ArgumentNullException(nameof(jsonOptions));
            this.JsonOptions = jsonOptions.Value;
        }

        private readonly string StoragePrefix;
        private readonly IJSRuntime JsRuntime;
        private JsonSerializerOptions JsonOptions;


        /// <summary>
        /// Returns the item with the specified <paramref name="key"/> from the underlying web storage.
        /// </summary>
        protected async Task<T?> GetItemAsync<T>(string key)
        {
            var val = await this.JsRuntime.InvokeAsync<string?>($"{this.StoragePrefix}.getItem", key);
            if(typeof(T) == typeof(string))
            {
                return (T?)(object?)val;
            }

            if(null != val)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(val ?? "", this.JsonOptions);
                }
                catch { }
            }

            return default;
        }

        /// <summary>
        /// Removes the item with the specified <paramref name="key"/> from the underlying web storage.
        /// </summary>
        protected async Task RemoveItemAsync(string key)
        {
            await this.RemoveKeyAsync(key);
            await this.JsRuntime.InvokeVoidAsync($"{this.StoragePrefix}.removeItem", key);
        }

        /// <summary>
        /// Stores the specified <paramref name="value"/> with the specified <paramref name="key"/> in the underlying web storage.
        /// </summary>
        protected async Task SetItemAsync(string key, object? value)
        {
            if(null != value)
            {
                await this.AddKeyAsync(key);
                await this.SetItemInternalAsync(key, value);
            }
            else
            {
                await this.RemoveItemAsync(key);
            }
        }

        /// <summary>
        /// Sets the specified item without tracking they key.
        /// </summary>
        protected async Task SetItemInternalAsync(string key, object? value)
        {
            if(null != value)
            {
                string storeValue;
                if(value is string)
                {
                    storeValue = (string)value;
                }
                else
                {
                    storeValue = JsonSerializer.Serialize(value, this.JsonOptions);
                }

                await this.JsRuntime.InvokeVoidAsync($"{this.StoragePrefix}.setItem", key, storeValue);
            }
            else
            {
                await this.RemoveItemAsync(key);
            }

        }

        private const string KeysKey = "keys";

        /// <summary>
        /// Returns the keys stored through this store.
        /// </summary>
        /// <returns></returns>
        protected async Task<IEnumerable<string>> GetKeysAsync()
        {
            var prefixedKey = this.GetKey(KeysKey);
            var keys = await this.GetItemAsync<string[]>(prefixedKey);
            return keys ?? Enumerable.Empty<string>();
        }

        private async Task AddKeyAsync(string key)
        {
            var keys = new List<string>(await this.GetKeysAsync());
            if (!keys.Contains(key))
            {
                keys.Add(key);
                await this.SetKeysAsync(keys);
            }

        }

        private async Task RemoveKeyAsync(string key)
        {
            var keys = new List<string>(await this.GetKeysAsync());
            if(keys.Contains(key))
            {
                keys.Remove(key);
                await this.SetKeysAsync(keys);
            }
        }

        private async Task SetKeysAsync(IEnumerable<string> keys)
        {
            var prefixedKey = this.GetKey(KeysKey);
            await this.SetItemInternalAsync(prefixedKey, keys.ToArray());
        }
    }
}
