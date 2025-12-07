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
    public class BlazorSessionTokenStore : TokenStoreBase
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
        public async override ValueTask<TokenContainer?> GetAccessTokenAsync()
        {
            return await this.GetContainerAsync(TokenType.AccessToken);
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
        public async override ValueTask SetAccessTokenAsync(TokenContainer token)
        {
            var key = this.GetKey(TokenType.AccessToken);
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
