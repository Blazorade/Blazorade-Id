using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
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
    /// A token store that stores tokens in the browser session storage.
    /// </summary>
    public class BlazorSessionTokenStore : TokenWebStoreBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlazorSessionTokenStore"/> class.
        /// </summary>
        public BlazorSessionTokenStore(ISessionStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private readonly ISessionStorageService Service;

        /// <inheritdoc/>
        public async override ValueTask<TokenContainer?> GetAccessTokenAsync(string resourceId)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            return await this.Service.GetItemAsync<TokenContainer?>(key);
        }

        /// <inheritdoc/>
        public async override ValueTask<TokenContainer?> GetIdentityTokenAsync()
        {
            return await this.GetContainerAsync(TokenType.IdentityToken);
        }

        /// <inheritdoc/>
        public async override ValueTask<TokenContainer?> GetRefreshTokenAsync()
        {
            return await this.GetContainerAsync(TokenType.RefreshToken);
        }

        /// <inheritdoc/>
        public async override ValueTask SetAccessTokenAsync(string resourceId, TokenContainer? token)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            await this.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async override ValueTask SetIdentityTokenAsync(TokenContainer? token)
        {
            var key = this.GetKey(TokenType.IdentityToken);
            await this.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async override ValueTask SetRefreshTokenAsync(TokenContainer? token)
        {
            var key = this.GetKey(TokenType.RefreshToken);
            await this.SetItemAsync(key, token);
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
