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
        public StorageFacade(IOptionsFactory<AuthorityOptions> optionsFactory, ISessionStorage sessionStorage, IPersistentStorage persistentStorage)
        {
            this.OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof (sessionStorage));
            this.PersistentStorage = persistentStorage ?? throw new ArgumentNullException(nameof(persistentStorage));
        }

        private readonly IOptionsFactory<AuthorityOptions> OptionsFactory;

        private const string CodeVerifierKey = "codeVerifier";
        private const string ScopeKey = "scope";
        private const string UsernameKey = "username";
        private const string AuthorityKey = "authKey";
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
        /// Returns the authority key for the current signed in user.
        /// </summary>
        public async ValueTask<string?> GetAuthorityKeyAsync()
        {
            var key = this.PrefixKey(AuthorityKey);
            return await this.PersistentStorage.GetItemAsync<string?>(key);
        }

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
            return await this.GetValueFromConfiguredStorageAsync<string?>(key);
        }

        /// <summary>
        /// Returns the current access token if it still is valid. If the token has expired, it will not be returned.
        /// </summary>
        public async ValueTask<JwtSecurityToken?> GetAccessTokenAsync()
        {
            var key = this.PrefixKey(AccessTokenKey);
            return await this.GetSecurityTokenFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Returns the current identity token if it still is valid. If the token has expired, it will not be returned.
        /// </summary>
        public async ValueTask<JwtSecurityToken?> GetIdentityTokenAsync()
        {
            var key = this.PrefixKey(IdTokenKey);
            return await this.GetSecurityTokenFromConfiguredStorageAsync(key);
        }

        /// <summary>
        /// Returns the current refresh token.
        /// </summary>
        public async ValueTask<string?> GetRefreshTokenAsync()
        {
            var key = this.PrefixKey(RefreshTokenKey);
            return await this.GetValueFromConfiguredStorageAsync<string?>(key);
        }


        /// <summary>
        /// Sets the authority key for the authority used to sign in the current user.
        /// </summary>
        public async ValueTask SetAuthorityKeyAsync(string? authorityKey)
        {
            var key = this.PrefixKey(AuthorityKey);
            await this.PersistentStorage.SetItemAsync(key, authorityKey);
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
            await this.SetValueInConfiguredStorageAsync(key, token);
        }

        /// <summary>
        /// Stores the identity token for the current user.
        /// </summary>
        public async ValueTask SetIdentityTokenAsync(TokenContainer token)
        {
            var key = this.PrefixKey(IdTokenKey);
            await this.SetValueInConfiguredStorageAsync(key, token);
        }

        /// <summary>
        /// Stores the refresh token for the current user.
        /// </summary>
        public async ValueTask SetRefreshTokenAsync(TokenContainer token)
        {
            var key = this.PrefixKey(RefreshTokenKey);
            await this.SetValueInConfiguredStorageAsync(key, token);
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
            await this.SetValueInConfiguredStorageAsync(key, username);
        }



        /// <summary>
        /// Removes the authority key that was used to sign the current user in.
        /// </summary>
        public async ValueTask RemoveAuthoritykeyAsync()
        {
            var key = this.PrefixKey(AuthorityKey);
            await this.PersistentStorage.RemoveItemAsync(key);
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
        /// Removes all items from all stores for the current user.
        /// </summary>
        /// <returns></returns>
        public async ValueTask RemoveItemsAsync()
        {
            await this.RemoveNonceAsync();
            await this.RemoveScopeAsync();
            await this.RemoveCodeVerifierAsync();



            await this.RemoveUsernameAsync();
            await this.RemoveAuthoritykeyAsync();
        }


        /// <summary>
        /// Returns the storage configured for the authority key currently in use, i.e. the
        /// authority key returned by the <see cref="GetCurrentAuthorityKeyAsync"/>.
        /// </summary>
        public async ValueTask<IStorage> GetConfiguredStorageAsync()
        {
            var authKey = await this.GetAuthorityKeyAsync();
            return this.GetConfiguredStorage(authKey);
        }

        /// <summary>
        /// Returns the storage configured for the authority represented by the given key.
        /// </summary>
        public IStorage GetConfiguredStorage(string? authorityKey)
        {
            var options = this.OptionsFactory.Create(authorityKey ?? "");
            return this.GetConfiguredStorage(options);
        }

        /// <summary>
        /// Returns the storage configured in the given options.
        /// </summary>
        public IStorage GetConfiguredStorage(AuthorityOptions options)
        {
            return options.CacheMode == TokenCacheMode.Session
                ? (IStorage)this.SessionStorage
                : (IStorage)this.PersistentStorage;
        }


        private async ValueTask<JwtSecurityToken?> GetSecurityTokenFromConfiguredStorageAsync(string key)
        {
            var container = await this.GetValueFromConfiguredStorageAsync<TokenContainer?>(key);
            if(null != container && (null == container.Expires || container.Expires > DateTime.UtcNow) && container.Token?.Length > 0)
            {
                return new JwtSecurityToken(container.Token);
            }
            return null;
        }

        private async ValueTask<T> GetValueFromConfiguredStorageAsync<T>(string key)
        {
            var storage = await this.GetConfiguredStorageAsync();
            return await storage.GetItemAsync<T>(key);
        }

        private string PrefixKey(string key, string? authKey = null)
        {
            return authKey?.Length > 0 ? $"blazorade.{authKey}.{key}" : $"blazorade.{key}";
        }

        private async ValueTask RemoveItemFromConfiguredStorageAsync(string key)
        {
            var storage = await this.GetConfiguredStorageAsync();
            await storage.RemoveItemAsync(key);
        }

        private async ValueTask SetValueInConfiguredStorageAsync<T>(string key, T value)
        {
            var storage = await this.GetConfiguredStorageAsync();
            await storage.SetItemAsync(key, value);
        }

    }
}
