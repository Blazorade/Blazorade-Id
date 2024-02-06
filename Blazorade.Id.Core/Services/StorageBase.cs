using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public abstract class StorageBase : IStorage
    {
        private const string ScopeKey = "scope";
        private const string CodeVerifierKey = "codeVerifier";
        private const string CurrentUsernameKey = "currentUsername";
        private const string CurrentAuthorityKey = "currentAuthKey";
        private const string NonceKey = "nonce";
        private const string TokenKey = "token";

        /// <inheritdoc/>
        public abstract ValueTask ClearAsync();

        /// <inheritdoc/>
        public abstract ValueTask<bool> ContainsKeyAsync(string key);

        /// <inheritdoc/>
        public virtual async ValueTask<string?> GetCurrentAuthorityKeyAsync()
        {
            var key = this.PrefixKey(CurrentAuthorityKey);
            return await this.GetItemAsync<string?>(key);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string?> GetCurrentCodeVerifierAsync()
        {
            var key = this.PrefixKey(CodeVerifierKey);
            return await this.GetItemAsync<string?>(key);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string?> GetCurrentNonceAsync()
        {
            var key = this.PrefixKey(NonceKey);
            return await this.GetItemAsync<string?>(key);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string?> GetCurrentScopeAsync()
        {
            var key = this.PrefixKey(ScopeKey);
            return await this.GetItemAsync<string?>(key);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TokenSet?> GetCurrentTokenSetAsync(bool includeExpired = false)
        {
            var authKey = await this.GetCurrentAuthorityKeyAsync();
            var username = await this.GetCurrentUsernameAsync();
            if(username?.Length > 0)
            {
                var key = this.CreateTokenSetKey(username, authKey);
                var set = await this.GetItemAsync<TokenSet>(key);
                if(includeExpired || set?.ExpiresAtUtc > DateTime.UtcNow)
                {
                    return set;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string?> GetCurrentUsernameAsync()
        {
            var key = this.PrefixKey(CurrentUsernameKey);
            return await this.GetItemAsync<string?>(key);
        }

        /// <inheritdoc/>
        public abstract ValueTask<T> GetItemAsync<T>(string key);

        /// <inheritdoc/>
        public abstract ValueTask<int> GetItemCountAsync();

        /// <inheritdoc/>
        public abstract ValueTask<IEnumerable<string>> GetKeysAsync();

        /// <inheritdoc/>
        public virtual async ValueTask RemoveCurrentCodeVerifierAsync()
        {
            var key = this.PrefixKey(CodeVerifierKey);
            await this.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveCurrentNonceAsync()
        {
            var key = this.PrefixKey(NonceKey);
            await this.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveCurrentScopeAsync()
        {
            var key = this.PrefixKey(ScopeKey);
            if(await this.ContainsKeyAsync(key))
            {
                await this.RemoveItemAsync(key);
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveCurrentTokenSetAsync()
        {
            var username = await this.GetCurrentUsernameAsync();
            if(username?.Length > 0)
            {
                var authKey = await this.GetCurrentAuthorityKeyAsync();
                var key = this.CreateTokenSetKey(username, authKey);
                await this.RemoveItemAsync(key);
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveCurrentUsernameAsync()
        {
            var key = this.PrefixKey(CurrentUsernameKey);
            await this.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public abstract ValueTask RemoveItemAsync(string key);

        /// <inheritdoc/>
        public virtual async ValueTask RemoveItemsAsync(IEnumerable<string> keys)
        {
            foreach(var key in keys ?? Enumerable.Empty<string>())
            {
                await this.RemoveItemAsync(key);
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetCurrentAuthorityKeyAsync(string? authorityKey)
        {
            var key = this.PrefixKey(CurrentAuthorityKey);
            await this.SetItemAsync(key, authorityKey);
        }

        /// <inheritdoc/>
        public async ValueTask SetCurrentCodeVerifierAsync(string? codeVerifier)
        {
            var key = this.PrefixKey(CodeVerifierKey);
            await this.SetItemAsync(key, codeVerifier);
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetCurrentNonceAsync(string? nonce)
        {
            var key = this.PrefixKey(NonceKey);
            await this.SetItemAsync(key, nonce);
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetCurrentScopeAsync(string? scope)
        {
            var key = this.PrefixKey(ScopeKey);
            await this.SetItemAsync(key, scope);
        }

        /// <inheritdoc/>
        public virtual async ValueTask SetCurrentUsernameAsync(string? username)
        {
            var key = this.PrefixKey(CurrentUsernameKey);
            await this.SetItemAsync(key, username);
        }

        /// <inheritdoc/>
        public async ValueTask SetCurrentTokenSetAsync(TokenSet? tokenSet)
        {
            var username = await this.GetCurrentUsernameAsync();
            if(username?.Length > 0)
            {
                var authKey = await this.GetCurrentAuthorityKeyAsync();
                var key = this.CreateTokenSetKey(username, authKey);
                await this.SetItemAsync(key, tokenSet);
            }
        }

        /// <inheritdoc/>
        public abstract ValueTask SetItemAsync<T>(string key, T value);



        /// <inheritdoc/>
        protected string PrefixKey(string key, string? authKey = null)
        {
            return authKey?.Length > 0 ? $"blazorade.{authKey}.{key}" : $"blazorade.{key}";
        }

        /// <inheritdoc/>
        protected string CreateTokenSetKey(string username, string? authorityKey = null)
        {
            return this.PrefixKey($"{username}.{TokenKey}", authorityKey);
        }

    }
}
