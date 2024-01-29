using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service implementation that takes care of acquiring tokens on behalf of the application
    /// it is used in.
    /// </summary>
    public class BlazoradeIdService
    {
        public BlazoradeIdService(IOptionsFactory<AuthorityOptions> optionsFactory, IHttpClientFactory clientFactory, EndpointService epService, ISessionStorage sessionStorage, IPersistentStorage persistentStorage, INavigator navigator)
        {
            this.OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.EPService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof(sessionStorage));
            this.PersistentStorage = persistentStorage ?? throw new ArgumentNullException(nameof(persistentStorage));
            this.Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        private readonly IOptionsFactory<AuthorityOptions> OptionsFactory;
        private readonly IHttpClientFactory ClientFactory;
        private readonly EndpointService EPService;
        private readonly ISessionStorage SessionStorage;
        private readonly IPersistentStorage PersistentStorage;
        private readonly INavigator Navigator;

        public async ValueTask<TokenSet?> GetTokenSetAsync(string scope = ".default", string? username = null, bool silent = false, string? authorityKey = null, bool useCurrentAuthorityKey = true)
        {
            var authKey = authorityKey?.Length > 0
                ? authorityKey
                : useCurrentAuthorityKey == true
                    ? await this.GetCurrentAuthorityKeyAsync()
                    : null;

            var login = username?.Length > 0 ? username : await this.GetCurrentLoginHintAsync();
            if(login?.Length > 0)
            {
                var storage = this.GetConfiguredStorage(authKey);
                var tokenKey = this.CreateTokenSetKey(authKey, login);
                var tokenSet = await storage.GetItemAsync<TokenSet>(tokenKey);
                if(tokenSet.ExpiresAtUtc > DateTime.UtcNow)
                {
                    return tokenSet;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the login hint, i.e. the username of the currently logged in user.
        /// </summary>
        /// <param name="authorityKey"></param>
        /// <returns></returns>
        public async ValueTask<string?> GetCurrentLoginHintAsync()
        {
            var authorityKey = await this.GetCurrentAuthorityKeyAsync();
            var key = this.CreateCurrentLoginHintStorageKey();
            var storage = this.GetConfiguredStorage(authorityKey);
            var loginHint = await storage.GetItemAsync<string>(key);

            return loginHint?.Length > 0 ? loginHint : null;
        }

        /// <summary>
        /// Returns the authority key that the currently logged on user was logged in with.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<string?> GetCurrentAuthorityKeyAsync()
        {
            var storageKey = this.CreateCurrentAuthorityKeyStorageKey();
            return await this.PersistentStorage.GetItemAsync<string>(storageKey);
        }


        public async ValueTask LogoutAsync(string? postLogoutRedirectUri = null, bool redirectToCurrentUri = true)
        {
            var authKeyKey = this.CreateCurrentAuthorityKeyStorageKey();
            var authorityKey = await this.PersistentStorage.GetItemAsync<string>(authKeyKey);

            var authOptions = this.GetAuthOptions(authorityKey);
            var builder = await this.EPService.CreateEndSessionUriBuilderAsync(authOptions);

            var logoutUri = builder
                .WithPostLogoutRedirectUri(
                    postLogoutRedirectUri?.Length > 0
                        ? postLogoutRedirectUri
                        : redirectToCurrentUri
                            ? this.Navigator.CurrentUri
                            : null
                )
                .Build();

            await this.Navigator.NavigateToAsync(logoutUri);
        }





        private string CreateCurrentLoginHintStorageKey()
        {
            return this.PrefixStorageKey(null, "currentLogin");
        }

        private string CreateCurrentAuthorityKeyStorageKey()
        {
            return this.PrefixStorageKey(null, "currentAuthKey");
        }

        private string CreateTokenSetKey(string? authKey, string username)
        {
            return PrefixStorageKey(authKey, $"{username}.tokenSet");
        }

        private AuthorityOptions GetAuthOptions(string? key)
        {
            var authOptions = this.OptionsFactory.Create(key ?? string.Empty);
            if (null == authOptions)
            {
                throw new ArgumentException("No authentication options found with the key specified in key.", nameof(key));
            }
            return authOptions;
        }

        private IStorage GetConfiguredStorage(AuthorityOptions authOptions)
        {
            return authOptions.CacheMode == TokenCacheMode.Session
                ? (IStorage)this.SessionStorage 
                : (IStorage)this.PersistentStorage;


        }

        private IStorage GetConfiguredStorage(string? key)
        {
            var options = this.GetAuthOptions(key);
            return this.GetConfiguredStorage(options);
        }

        private string PrefixStorageKey(string? authKey, string key)
        {
            return authKey?.Length > 0 ? $"blazorade.{authKey}.{key}" : $"blazorade.{key}";
        }

    }

}
