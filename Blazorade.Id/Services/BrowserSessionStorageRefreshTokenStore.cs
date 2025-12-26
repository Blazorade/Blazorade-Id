using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A refresh token store that stores refresh tokens in the browser's session storage.
    /// </summary>
    /// <remarks>
    /// Storing tokens in a browser's session storage makes them more vulnerable to XSS attacks.
    /// This is especially true for refresh tokens, which are long-lived tokens that can be used 
    /// to obtain new access tokens.
    /// </remarks>
    public class BrowserSessionStorageRefreshTokenStore : StoreBase, IRefreshTokenStore
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public BrowserSessionStorageRefreshTokenStore(ISessionStorageService service)
        {
            this.Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private ISessionStorageService Service;

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            var key = this.GetKey(Model.TokenType.RefreshToken);
            if(await this.Service.ContainKeyAsync(key))
            {
                await this.Service.RemoveItemAsync(key);
            }
        }

        /// <inheritdoc/>
        public async Task<string?> GetRefreshTokenAsync()
        {
            var key = this.GetKey(Model.TokenType.RefreshToken);
            if(await this.Service.ContainKeyAsync(key))
            {
                return await this.Service.GetItemAsync<string>(key);
            }
            return null;
        }

        /// <inheritdoc/>
        public async Task SetRefreshTokenAsync(string? token)
        {
            var key = this.GetKey(Model.TokenType.RefreshToken);
            if (null != token)
            {
                await this.Service.SetItemAsync(key, token);
            }
            else
            {
                await this.ClearAsync();
            }
        }
    }
}
