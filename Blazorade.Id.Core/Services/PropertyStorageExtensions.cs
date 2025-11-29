using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Extension methods for <see cref="IPropertyStore"/> implementations.
    /// </summary>
    public static class PropertyStorageExtensions
    {

        private const string CodeVerifierKey = "codeVerifier";
        private const string ScopeKey = "scope";
        private const string UsernameKey = "username";
        private const string NonceKey = "nonce";

        /// <summary>
        /// Returns the code verifier that was used when starting the current login process.
        /// </summary>
        public static async ValueTask<string?> GetCodeVerifierAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(CodeVerifierKey);
            return await storage.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Returns the nonce that was used when starting the current login process.
        /// </summary>
        public static async ValueTask<string?> GetNonceAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(NonceKey);
            return await storage.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Returns the scope that was used when starting the current login process.
        /// </summary>
        public static async ValueTask<string?> GetScopeAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(ScopeKey);
            return await storage.GetPropertyAsync<string?>(key);
        }

        /// <summary>
        /// Returns the username for the current signed in user.
        /// </summary>
        public static async ValueTask<string?> GetUsernameAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(UsernameKey);
            return await storage.GetPropertyAsync<string?>(key);
        }



        /// <summary>
        /// Sets the code verifier that was used when starting the current login process.
        /// </summary>
        public static async ValueTask SetCodeVerifierAsync(this IPropertyStore storage, string? codeVerifier)
        {
            var key = PrefixKey(CodeVerifierKey);
            await storage.SetPropertyAsync(key, codeVerifier);
        }

        /// <summary>
        /// Sets the nonce that was used when starting the current login process.
        /// </summary>
        public static async ValueTask SetNonceAsync(this IPropertyStore storage, string? nonce)
        {
            var key = PrefixKey(NonceKey);
            await storage.SetPropertyAsync(key, nonce);
        }

        /// <summary>
        /// Sets the scope that was used when starting the current login process.
        /// </summary>
        public static async ValueTask SetScopeAsync(this IPropertyStore storage, string? scope)
        {
            var key = PrefixKey(ScopeKey);
            await storage.SetPropertyAsync(key, scope);
        }

        /// <summary>
        /// Sets the username for the current signed in user.
        /// </summary>
        public static async ValueTask SetUsernameAsync(this IPropertyStore storage, string? username)
        {
            var key = PrefixKey(UsernameKey);
            await storage.SetPropertyAsync(key, username);
        }



        /// <summary>
        /// Removes the code verifier that was used when starting the current login process.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask RemoveCodeVerifierAsync(this IPropertyStore storage)
        {
            var key =  PrefixKey(CodeVerifierKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Sets the nonce that was used when starting the current login process.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask RemoveNonceAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(NonceKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Sets the scope that was used when starting the current login process.
        /// </summary>
        public static async ValueTask RemoveScopeAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(ScopeKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Removes the username for the current signed in user, for instance when logging out.
        /// </summary>
        public static async ValueTask RemoveUsernameAsync(this IPropertyStore storage)
        {
            var key = PrefixKey(UsernameKey);
            await storage.RemovePropertyAsync(key);
        }

        /// <summary>
        /// Removes all items from all stores for the current user.
        /// </summary>
        /// <returns></returns>
        public static async ValueTask RemoveItemsAsync(this IPropertyStore storage)
        {
            await storage.RemoveCodeVerifierAsync();
            await storage.RemoveNonceAsync();
            await storage.RemoveScopeAsync();
            await storage.RemoveUsernameAsync();
        }

        private static string PrefixKey(string key, string? authKey = null)
        {
            return authKey?.Length > 0 ? $"blazorade.{authKey}.{key}" : $"blazorade.{key}";
        }

    }
}
