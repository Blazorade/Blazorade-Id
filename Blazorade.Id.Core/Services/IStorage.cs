using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{

    /// <summary>
    /// Defines the common interface implemented by different storages.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Determines whether an item with the given key exists in the storage.
        /// </summary>
        /// <param name="key">The key to check.</param>
        ValueTask<bool> ContainsKeyAsync(string key);

        /// <summary>
        /// Returns an item from the key if it exists.
        /// </summary>
        /// <typeparam name="T">The type to return the item as.</typeparam>
        /// <param name="key">The key of the item to return.</param>
        ValueTask<T> GetItemAsync<T>(string key);

        /// <summary>
        /// Removes the item with the given key from the storage.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ValueTask RemoveItemAsync(string key);

        /// <summary>
        /// Removes the items matching the given keys.
        /// </summary>
        /// <param name="keys">The keys of the items to remove.</param>
        ValueTask RemoveItemsAsync(IEnumerable<string> keys);

        /// <summary>
        /// Stores an item in the storage.
        /// </summary>
        /// <typeparam name="T">The type of item to store.</typeparam>
        /// <param name="key">The key to store the item with.</param>
        /// <param name="value">The item to store.</param>
        ValueTask SetItemAsync<T>(string key, T value);
    }

    /// <summary>
    /// Represents the session storage.
    /// </summary>
    public interface ISessionStorage : IStorage
    {
    }

    /// <summary>
    /// Represents the local storage that is persisted across sessions.
    /// </summary>
    public interface IPersistentStorage : IStorage
    {
    }
}
