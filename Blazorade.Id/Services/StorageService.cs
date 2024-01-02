using Blazorade.Id.Configuration;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Acts as a wrapper for both local and session storage.
    /// </summary>
    public class StorageService
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <exception cref="ArgumentNullException">The exception that is throw when any of the parameters are <c>null</c>.</exception>
        public StorageService(IOptionsFactory<BlazoradeAuthenticationOptions> factory, ILocalStorageService localStorage, ISessionStorageService sessionStorage)
        {
            this.OptionsFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.LocalStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
        }

        private const string KeyPrefix = "Blazorade.";
        internal const string IdentityToken = $"{KeyPrefix}{nameof(IdentityToken)}";
        internal const string AccessToken = $"{KeyPrefix}{nameof(AccessToken)}";
        internal const string RefreshToken = $"{KeyPrefix}{nameof(RefreshToken)}";
        internal const string LoginHint = $"{KeyPrefix}{nameof(LoginHint)}";

        private readonly IOptionsFactory<BlazoradeAuthenticationOptions> OptionsFactory;
        private readonly ILocalStorageService LocalStorage;
        private readonly ISessionStorageService SessionStorage;

        /// <summary>
        /// Returns the item with the given key from the storage specified in the default options.
        /// </summary>
        /// <typeparam name="T">The type to return the item as.</typeparam>
        /// <param name="key">The key of the item to return.</param>
        public async Task<T> GetItemAsync<T>(string key)
        {
            var options = this.OptionsFactory.Create("");
            return await this.GetItemAsync<T>(options, key);
        }

        /// <summary>
        /// Returns the item with the given key from the storage specified by the named options.
        /// </summary>
        /// <typeparam name="T">The type to return the item as.</typeparam>
        /// <param name="optionsName">The name of the options that define the storage to use.</param>
        /// <param name="key">The key of the item to return.</param>
        public async Task<T> GetItemAsync<T>(string optionsName, string key)
        {
            var options = this.OptionsFactory.Create(optionsName);
            return await this.GetItemAsync<T>(options, key);
        }

        /// <summary>
        /// Returns the item with the given key from the storage defined in <paramref name="options"/>.
        /// </summary>
        /// <typeparam name="T">The type to return the item as.</typeparam>
        /// <param name="options">The options that define what storage to use.</param>
        /// <param name="key">The key of the item to return.</param>
        /// <returns></returns>
        public async Task<T> GetItemAsync<T>(BlazoradeAuthenticationOptions? options, string key)
        {
            T item = default!;
            if (options?.CacheMode == TokenCacheMode.Persistent)
            {
                item = await this.LocalStorage.GetItemAsync<T>(key);
            }
            else if (options?.CacheMode == TokenCacheMode.Session)
            {
                item = await this.SessionStorage.GetItemAsync<T>(key);
            }
            return item;
        }

        /// <summary>
        /// Stores the item in the storage defined in the default options.
        /// </summary>
        /// <typeparam name="T">The type of the item to store.</typeparam>
        /// <param name="key">The key to store the item with.</param>
        /// <param name="item">The item to store.</param>
        public async Task SetItemAsync<T>(string key, T item)
        {
            var options = this.OptionsFactory.Create("");
            await this.SetItemAsync<T>(options, key, item);
        }

        /// <summary>
        /// Stores the item in the storage defined by the named options.
        /// </summary>
        /// <typeparam name="T">The type of the item to store.</typeparam>
        /// <param name="optionsName">The name of the options that define the storage to use.</param>
        /// <param name="key">The key to store the item with.</param>
        /// <param name="item">The item to store.</param>
        public async Task SetItemAsync<T>(string optionsName, string key, T item)
        {
            var options = this.OptionsFactory.Create(optionsName);
            await this.SetItemAsync<T>(options, key, item);
        }

        /// <summary>
        /// Stores the item in the storage defined by <paramref name="options"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item to store.</typeparam>
        /// <param name="options">The options that define what storage to use.</param>
        /// <param name="key">The key to store the item with.</param>
        /// <param name="item">The item to store.</param>
        public async Task SetItemAsync<T>(BlazoradeAuthenticationOptions? options, string key, T item)
        {
            if (options?.CacheMode == TokenCacheMode.Persistent)
            {
                await this.LocalStorage.SetItemAsync(key, item);
            }
            else if (options?.CacheMode == TokenCacheMode.Session)
            {
                await this.SessionStorage.SetItemAsync(key, item);
            }
        }

        /// <summary>
        /// Removes the item with the given key from the storage defined in the default options.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        public async Task RemoveItemAsync(string key)
        {
            var options = this.OptionsFactory.Create("");
            await this.RemoveItemAsync(options, key);
        }

        /// <summary>
        /// Removes the item with the given key from the storage defined in options with the given name.
        /// </summary>
        /// <param name="optionsName">The name of the options that define the storage to use.</param>
        /// <param name="key">The key of the item to remove.</param>
        public async Task RemoveItemAsync(string optionsName, string key)
        {
            var options = this.OptionsFactory.Create(optionsName);
            await this.RemoveItemAsync(options, key);
        }

        /// <summary>
        /// Removes the item with the given key from the storage defined by <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options that define the storage to use.</param>
        /// <param name="key">The key of the item to remove.</param>
        public async Task RemoveItemAsync(BlazoradeAuthenticationOptions? options, string key)
        {
            if (options?.CacheMode == TokenCacheMode.Persistent)
            {
                await this.LocalStorage.RemoveItemAsync(key);
            }
            else if (options?.CacheMode == TokenCacheMode.Session)
            {
                await this.SessionStorage.RemoveItemAsync(key);
            }
        }

        /// <summary>
        /// Removes all specified items from the storage defined by the default options.
        /// </summary>
        /// <param name="keys">The keys of the items to remove.</param>
        public async Task RemoveItemsAsync(IEnumerable<string> keys)
        {
            var options = this.OptionsFactory.Create("");
            foreach (var key in keys)
            {
                await this.RemoveItemAsync(options, key);
            }
        }

        /// <summary>
        /// Removes all specified items from the storage defined in the options with the given name.
        /// </summary>
        /// <param name="optionsName">The name of the options that define the storage to use.</param>
        /// <param name="keys">The keys of the items to remove.</param>
        public async Task RemoveItemsAsync(string optionsName, IEnumerable<string> keys)
        {
            var options = this.OptionsFactory.Create(optionsName);
            foreach(var key in keys)
            {
                await this.RemoveItemAsync(options, key);
            }
        }

    }
}
