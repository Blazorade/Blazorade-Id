using Blazorade.Id.Model;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A base class for token store implementations that use web storage in a browser.
    /// </summary>
    public abstract class WebTokenStoreBase : WebStoreBase, ITokenStore
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        protected WebTokenStoreBase(WebStoreType storeType, IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(storeType, jsRuntime, jsonOptions)
        {
        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            var keys = await this.GetKeysAsync();
            foreach (var key in keys)
            {
                await this.RemoveItemAsync(key);
            }
        }

        /// <inheritdoc/>
        public async Task<TokenContainer?> GetAccessTokenAsync(string resourceId)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            return await this.GetItemAsync<TokenContainer?>(key);
        }

        /// <inheritdoc/>
        public async Task<TokenContainer?> GetIdentityTokenAsync()
        {
            var key = this.GetKey(TokenType.IdentityToken);
            return await this.GetItemAsync<TokenContainer?>(key);
        }

        /// <inheritdoc/>
        public async Task SetAccessTokenAsync(string resourceId, TokenContainer? token)
        {
            var key = this.GetKey(TokenType.AccessToken, suffix: resourceId);
            await this.SetItemAsync(key, token);
        }

        /// <inheritdoc/>
        public async Task SetIdentityTokenAsync(TokenContainer? token)
        {
            var key = this.GetKey(TokenType.IdentityToken);
            await this.SetItemAsync(key, token);
        }
    }
}
