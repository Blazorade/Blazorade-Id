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
        /// Clears all items from the storage.
        /// </summary>
        /// <returns></returns>
        ValueTask ClearAsync();

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
        /// Returns a collection of keys representing the items stored in the storage.
        /// </summary>
        /// <returns></returns>
        ValueTask<IEnumerable<string>> GetKeysAsync();

        /// <summary>
        /// Returns the number of items stored in the storage.
        /// </summary>
        /// <returns></returns>
        ValueTask<int> GetItemCountAsync();

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

        /// <summary>
        /// Returns the scope for the current users' tokens.
        /// </summary>
        ValueTask<string?> GetCurrentScopeAsync();

        /// <summary>
        /// Returns the current nonce so that it can be verified when receiving an identity token.
        /// </summary>
        ValueTask<string?> GetCurrentNonceAsync();

        /// <summary>
        /// Returns the current code verifier. Use it to send token requests to the token endpoint.
        /// </summary>
        ValueTask<string?> GetCurrentCodeVerifierAsync();

        /// <summary>
        /// Returns the username for the user currently signed in.
        /// </summary>
        ValueTask<string?> GetCurrentUsernameAsync();

        /// <summary>
        /// Returns the authority key that was used to log in the current user.
        /// </summary>
        ValueTask<string?> GetCurrentAuthorityKeyAsync();

        ValueTask<TokenSet?> GetCurrentTokenSetAsync(bool includeExpired = false);

        /// <summary>
        /// Removes the scope for the current users's tokens.
        /// </summary>
        /// <remarks>
        /// Removing the scope does not remove anything else from the store.
        /// </remarks>
        ValueTask RemoveCurrentScopeAsync();

        /// <summary>
        /// Removes the current nonce.
        /// </summary>
        /// <returns></returns>
        ValueTask RemoveCurrentNonceAsync();

        /// <summary>
        /// Removes the current code verifier.
        /// </summary>
        /// <remarks>
        /// After the code verifier has been removed, tokens cannot be acquired using an authorization code. Remove this
        /// only after you have acquired a refresh token that you can use.
        /// </remarks>
        ValueTask RemoveCurrentCodeVerifierAsync();

        /// <summary>
        /// Removes the current username, for instance when a user logs out.
        /// </summary>
        ValueTask RemoveCurrentUsernameAsync();

        /// <summary>
        /// Removes the current token set, for instance when a user logs out.
        /// </summary>
        ValueTask RemoveCurrentTokenSetAsync();

        /// <summary>
        /// Stores the scope for the current users' tokens.
        /// </summary>
        /// <param name="scope">The scope to store.</param>
        ValueTask SetCurrentScopeAsync(string? scope);

        /// <summary>
        /// Stores the given nonce for the current user so that it can be verified when the identity token is received.
        /// </summary>
        ValueTask SetCurrentNonceAsync(string? nonce);

        /// <summary>
        /// Sets the current code verifier so that it can be used when acquiring tokens with an authorization code.
        /// </summary>
        ValueTask SetCurrentCodeVerifierAsync(string? codeVerifier);

        /// <summary>
        /// Sets the username for the user currently signed in.
        /// </summary>
        ValueTask SetCurrentUsernameAsync(string? username);

        /// <summary>
        /// Sets the authority key that defines the authority options that were used to sign in the current user.
        /// </summary>
        ValueTask SetCurrentAuthorityKeyAsync(string? authorityKey);

        /// <summary>
        /// Sets the token set for the current user.
        /// </summary>
        ValueTask SetCurrentTokenSetAsync(TokenSet? tokenSet);
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
