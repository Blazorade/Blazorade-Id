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
    /// A base implementation for refresh token stores that use web storage in a browser.
    /// </summary>
    public abstract class WebRefreshTokenStoreBase : WebStoreBase, IRefreshTokenStore
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        protected WebRefreshTokenStoreBase (WebStoreType storeType, IJSRuntime jsRuntime, IOptions<JsonSerializerOptions> jsonOptions) : base(storeType, jsRuntime, jsonOptions)
        {

        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            var key = this.GetKey(Model.TokenType.RefreshToken);
            await this.RemoveItemAsync(key);
        }

        /// <inheritdoc/>
        public async Task<string?> GetRefreshTokenAsync()
        {
            var key = this.GetKey(Model.TokenType.RefreshToken);
            return await this.GetItemAsync<string?>(key);
        }

        /// <inheritdoc/>
        public async Task SetRefreshTokenAsync(string? token)
        {
            var key = this.GetKey(Model.TokenType.RefreshToken);
            await this.SetItemAsync(key, token);
        }
    }
}
