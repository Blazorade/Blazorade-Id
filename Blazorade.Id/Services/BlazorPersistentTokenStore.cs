using Blazorade.Id.Core.Model;
using Blazorade.Id.Core.Services;
using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A token store that uses the browser local storage to persist tokens across sessions.
    /// </summary>
    public class BlazorPersistentTokenStore : TokenStoreBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BlazorPersistentTokenStore"/> class.
        /// </summary>
        public BlazorPersistentTokenStore(ILocalStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private ILocalStorageService Service;

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
        public async override ValueTask SetAccessTokenAsync(string resourceId, TokenContainer token)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            await this.Service.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async override ValueTask SetIdentityTokenAsync(TokenContainer token)
        {
            var key = this.GetKey(TokenType.IdentityToken);
            await this.Service.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async override ValueTask SetRefreshTokenAsync(TokenContainer token)
        {
            var key = this.GetKey(TokenType.RefreshToken);
            await this.Service.SetItemAsync(key, token);
        }



        private async ValueTask<TokenContainer?> GetContainerAsync(TokenType tokenType)
        {
            var key = this.GetKey(tokenType);
            return await this.Service.GetItemAsync<TokenContainer?>(key);
        }

    }
}
