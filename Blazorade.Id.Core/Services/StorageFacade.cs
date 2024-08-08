using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A storage factory service implementation.
    /// </summary>
    /// <remarks>
    /// The storage factory is responsible for proving an application with
    /// the storage that is configured for each authority configured for the application.
    /// </remarks>
    public class StorageFacade
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// The exception that is thrown if one of the parameters is <c>null</c>.
        /// </exception>
        public StorageFacade(IOptions<AuthorityOptions> options, ISessionStorage sessionStorage, IPersistentStorage persistentStorage)
        {
            this.Options = options.Value ?? throw new ArgumentNullException(nameof(options));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof (sessionStorage));
            this.PersistentStorage = persistentStorage ?? throw new ArgumentNullException(nameof(persistentStorage));
        }

        private readonly AuthorityOptions Options;

        private const string CodeVerifierKey = "codeVerifier";
        private const string ScopeKey = "scope";
        private const string UsernameKey = "username";
        private const string NonceKey = "nonce";
        private const string RefreshTokenKey = "refreshToken";
        private const string AccessTokenKey = "accessToken";
        private const string IdTokenKey = "idToken";



        /// <summary>
        /// Returns the session storage configured for the application.
        /// </summary>
        public ISessionStorage SessionStorage { get; private set; }

        /// <summary>
        /// Returns the persistent storage configured for the application.
        /// </summary>
        public IPersistentStorage PersistentStorage { get; private set; }



        /// <summary>
        /// Returns the code verifier that was used when starting the current login process.
        /// </summary>
        public async ValueTask<string?> GetCodeVerifierAsync()
        {
            var key = this.PrefixKey(CodeVerifierKey);
            return await this.SessionStorage.GetItemAsync<string?>(key);
        }

        /// <summary>
        /// Returns the nonce that was used when starting the current login process.
        /// </summary>
        public async ValueTask<string?> GetNonceAsync()
        {
            var key = this.PrefixKey(NonceKey);
            return await this.SessionStorage.GetItemAsync<string?>(key);
        }

        /// <summary>
        /// Returns the scope that was used when starting the current login process.
        /// </summary>
        public async ValueTask<string?> GetScopeAsync()
        {
            var key = this.PrefixKey(ScopeKey);
            return await this.SessionStorage.GetItemAsync<string?>(key);
        }

        /// <summary>
        /// Returns the username for the current signed in user.
        /// </summary>
        public async ValueTask<string?> GetUsernameAsync()
        {
            var key = this.PrefixKey(UsernameKey);
            return await this.GetItemFromConfiguredStorageAsync<string?>(key);
        }

        /// <summary>
        /// Returns the current access token from storage.
        /// </summary>
        /// <remarks>
        /// This method does not examine the expiration for the token. If you want to
        /// make sure you have a valid token, use the <see cref="TokenService.GetAccessTokenAsync"/> method.
        /// </remarks>
        public async ValueTask<TokenContainer?> GetAccessTokenAsync()
        {
            var key = this.PrefixKey(AccessTokenKey);
            return await this.GetItemFromConfiguredStorageAsync<TokenContainer?>(key);
        }

        /// <summary>
        /// Returns the current identity token from storage.
        /// </summary>
        /// <remarks>
        /// This method does not examine the expiration for the token. If you want to
        /// make sure you have a valid token, use the <see cref="TokenService.GetIdentityTokenAsync"/> method.
        /// </remarks>
        public async ValueTask<TokenContainer?> GetIdentityTokenAsync()
        {
            var key = this.PrefixKey(IdTokenKey);
            return await this.GetItemFromConfiguredStorageAsync<TokenContainer?>(key);
        }

        /// <summary>
        /// Returns the current refresh token.
        /// </summary>
        public async ValueTask<TokenContainer?> GetRefreshTokenAsync()
        {
            var key = this.PrefixKey(RefreshTokenKey);
            return await this.GetItemFromConfiguredStorageAsync<TokenContainer?>(key);
        }


        /// <summary>
        /// Sets the code verifier that was used when starting the current login process.
        /// </summary>
        public async ValueTask SetCodeVerifierAsync(string? codeVerifier)
        {
            var key = this.PrefixKey(CodeVerifierKey);
            await this.SessionStorage.SetItemAsync(key, codeVerifier);
        }

        /// <summary>
        /// Sets the nonce that was used when starting the current login process.
        /// </summary>
        public async ValueTask SetNonceAsync(string? nonce)
        {
            var key = this.PrefixKey(NonceKey);
            await this.SessionStorage.SetItemAsync(key, nonce);
        }

        /// <summary>
        /// Stores the access token for the current user.
        /// </summary>
        public async ValueTask SetAccessTokenAsync(TokenContainer token)
        {
            var key = this.PrefixKey(AccessTokenKey);
            await this.SetItemInConfiguredStorageAsync(key, token);
        }

        /// <summary>
        /// Stores the identity token for the current user.
        /// </summary>
        public async ValueTask SetIdentityTokenAsync(TokenContainer token)
        {
            var key = this.PrefixKey(IdTokenKey);
            await this.SetItemInConfiguredStorageAsync(key, token);
        }

        /// <summary>
        /// Stores the refresh token for the current user.
        /// </summary>
        public async ValueTask SetRefreshTokenAsync(TokenContainer token)
        {
            var key = this.PrefixKey(RefreshTokenKey);
            await this.SetItemInConfiguredStorageAsync(key, token);
        }

        /// <summary>
        /// Sets the scope that was used when starting the current login process.
        /// </summary>
        public async ValueTask SetScopeAsync(string? scope)
        {
            var key = this.PrefixKey(ScopeKey);
            await this.SessionStorage.SetItemAsync(key, scope);
        }

        /// <summary>
        /// Sets the username for the current signed in user.
        /// </summary>
        public async ValueTask SetUsernameAsync(string? username)
        {
            var key = this.PrefixKey(UsernameKey);
            await this.SetItemInConfiguredStorageAsync(key, username);
        }



        /// <summary>
        /// Removes the code verifier that was used when starting the current login process.
        /// </summary>
        /// <returns></returns>
        public async ValueTask RemoveCodeVerifierAsync()
        {
            var key = this.PrefixKey(CodeVerifierKey);
            await this.RemoveItemFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Sets the nonce that was used when starting the current login process.
        /// </summary>
        /// <returns></returns>
        public async ValueTask RemoveNonceAsync()
        {
            var key = this.PrefixKey(NonceKey);
            await this.SessionStorage.RemoveItemAsync(key);
        }

        /// <summary>
        /// Sets the scope that was used when starting the current login process.
        /// </summary>
        public async ValueTask RemoveScopeAsync()
        {
            var key = this.PrefixKey(ScopeKey);
            await this.SessionStorage.RemoveItemAsync(key);
        }

        /// <summary>
        /// Removes the username for the current signed in user, for instance when logging out.
        /// </summary>
        public async ValueTask RemoveUsernameAsync()
        {
            var key = this.PrefixKey(UsernameKey);
            await this.RemoveItemFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Removes the access token for the current user.
        /// </summary>
        public async ValueTask RemoveAccessTokenAsync()
        {
            var key = this.PrefixKey(AccessTokenKey);
            await this.RemoveItemFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Removes the identity token stored for the current user.
        /// </summary>
        public async ValueTask RemoveIdentityTokenAsync()
        {
            var key = this.PrefixKey(IdTokenKey);
            await this.RemoveItemFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Removes the refresh token stored for the current user.
        /// </summary>
        public async ValueTask RemoveRefreshTokenAsync()
        {
            var key = this.PrefixKey(RefreshTokenKey);
            await this.RemoveItemFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Removes all items from all stores for the current user.
        /// </summary>
        /// <returns></returns>
        public async ValueTask RemoveItemsAsync()
        {
            await this.RemoveNonceAsync();
            await this.RemoveScopeAsync();
            await this.RemoveCodeVerifierAsync();

            await this.RemoveIdentityTokenAsync();
            await this.RemoveAccessTokenAsync();
            await this.RemoveRefreshTokenAsync();

            await this.RemoveUsernameAsync();
        }

        /// <summary>
        /// Returns the storage configured in the given options.
        /// </summary>
        public IStorage GetConfiguredStorage()
        {
            return this.Options.CacheMode == TokenCacheMode.Session
                ? (IStorage)this.SessionStorage
                : (IStorage)this.PersistentStorage;
        }



        private async ValueTask<T> GetItemFromConfiguredStorageAsync<T>(string key)
        {
            var storage = this.GetConfiguredStorage();
            return await storage.GetItemAsync<T>(key);
        }

        private string PrefixKey(string key, string? authKey = null)
        {
            return authKey?.Length > 0 ? $"blazorade.{authKey}.{key}" : $"blazorade.{key}";
        }

        private async ValueTask RemoveItemFromConfiguredStorageAsync(string key)
        {
            var storage = this.GetConfiguredStorage();
            await storage.RemoveItemAsync(key);
        }

        private async ValueTask SetItemInConfiguredStorageAsync<T>(string key, T value)
        {
            var storage = this.GetConfiguredStorage();
            await storage.SetItemAsync(key, value);
        }

    }
}
