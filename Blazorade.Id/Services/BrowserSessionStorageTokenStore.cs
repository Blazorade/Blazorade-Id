using Blazorade.Id.Model;
using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A token store that stores tokens in the browser's session storage.
    /// </summary>
    public class BrowserSessionStorageTokenStore : TokenWebStoreBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BrowserSessionStorageTokenStore"/> class.
        /// </summary>
        public BrowserSessionStorageTokenStore(ISessionStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ISessionStorageService Service;

        /// <inheritdoc/>
        public override async Task ClearAllAsync()
        {
            var keyPrefix = this.GetKeyPrefix();
            var keys = await this.Service.KeysAsync();
            foreach (var key in from x in keys where x.StartsWith(keyPrefix) select x)
            {
                await this.Service.RemoveItemAsync(key);
            }

            await this.InMemoryStore.ClearAllAsync();
        }

        /// <inheritdoc/>
        public async override Task<TokenContainer?> GetAccessTokenAsync(string resourceId)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            return await this.Service.GetItemAsync<TokenContainer?>(key);
        }

        /// <inheritdoc/>
        public async override Task<TokenContainer?> GetIdentityTokenAsync()
        {
            return await this.GetContainerAsync(TokenType.IdentityToken);
        }

        /// <inheritdoc/>
        public async override Task<TokenContainer?> GetRefreshTokenAsync()
        {
            if (this.AllowRefreshTokensInWebStorage)
            {
                return await this.GetContainerAsync(TokenType.RefreshToken);
            }
            else
            {
                return await this.InMemoryStore.GetRefreshTokenAsync();
            }
        }

        /// <inheritdoc/>
        public async override Task SetAccessTokenAsync(string resourceId, TokenContainer? token)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            await this.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async override Task SetIdentityTokenAsync(TokenContainer? token)
        {
            var key = this.GetKey(TokenType.IdentityToken);
            await this.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async override Task SetRefreshTokenAsync(TokenContainer? token)
        {
            if(this.StoreRefreshTokens)
            {
                if (this.AllowRefreshTokensInWebStorage)
                {
                    var key = this.GetKey(TokenType.RefreshToken);
                    await this.SetItemAsync(key, token);
                }
                else
                {
                    await this.InMemoryStore.SetRefreshTokenAsync(token);
                }
            }
        }



        private async ValueTask<TokenContainer?> GetContainerAsync(TokenType tokenType)
        {
            var key = this.GetKey(tokenType);
            return await this.Service.GetItemAsync<TokenContainer?>(key);
        }

        private async Task SetItemAsync(string key, TokenContainer? token)
        {
            if (null != token)
            {
                await this.Service.SetItemAsync(key, token);
            }
            else if (await this.Service.ContainKeyAsync(key))
            {
                await this.Service.RemoveItemAsync(key);
            }
        }
    }
}
