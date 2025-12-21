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
    public interface IPropertyStore
    {
        /// <summary>
        /// Determines whether an item with the given key exists in the storage.
        /// </summary>
        /// <param name="key">The key to check.</param>
        Task<bool> ContainsKeyAsync(string key);

        /// <summary>
        /// Returns an property from the key if it exists.
        /// </summary>
        /// <typeparam name="T">The type to return the property as.</typeparam>
        /// <param name="key">The key of the property to return.</param>
        /// <returns>
        /// Returns the property if it exists. If the property does not exist, the default value of the type MUST be returned.
        /// Implementations that need to know if a property exists or not should use <see cref="ContainsKeyAsync"/> method first.
        /// </returns>
        Task<T> GetPropertyAsync<T>(string key);

        /// <summary>
        /// Removes the property with the given key from the storage.
        /// </summary>
        /// <param name="key">The key of the property to remove.</param>
        /// <returns></returns>
        Task RemovePropertyAsync(string key);

        /// <summary>
        /// Removes the properties matching the given keys.
        /// </summary>
        /// <param name="keys">The keys of the properties to remove.</param>
        Task RemovePropertiesAsync(IEnumerable<string> keys);

        /// <summary>
        /// Stores a property in the storage.
        /// </summary>
        /// <typeparam name="T">The type of property to store.</typeparam>
        /// <param name="key">The key to store the property with.</param>
        /// <param name="value">The property to store.</param>
        Task SetPropertyAsync<T>(string key, T value);
    }

}
