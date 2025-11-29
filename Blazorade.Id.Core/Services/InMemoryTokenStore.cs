using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class InMemoryTokenStore : ITokenStore
    {
        private TokenContainer? AccessToken;
        private TokenContainer? IdentityToken;
        private TokenContainer? RefreshToken;

        public ValueTask<TokenContainer?> GetAccessTokenAsync()
        {
            if(this.AccessToken?.Expires > DateTime.UtcNow)
            {
                return ValueTask.FromResult<TokenContainer?>(this.AccessToken);
            }

            return ValueTask.FromResult<TokenContainer?>(null);
        }

        public ValueTask<TokenContainer?> GetIdentityTokenAsync()
        {
            if(this.IdentityToken?.Expires > DateTime.UtcNow)
            {
                return ValueTask.FromResult<TokenContainer?>(this.IdentityToken);
            }

            return ValueTask.FromResult<TokenContainer?>(null);
        }

        public ValueTask<TokenContainer?> GetRefreshTokenAsync()
        {
            if(null == this.RefreshToken?.Expires || this.RefreshToken?.Expires > DateTime.UtcNow)
            {
                return ValueTask.FromResult<TokenContainer?>(this.RefreshToken);
            }

            return ValueTask.FromResult<TokenContainer?>(null);
        }

        public async ValueTask<TokenContainer?> SetAccessTokenAsync(string token)
        {
            var jwt = new JwtSecurityToken(token);
            return await this.SetAccessTokenAsync(jwt);
        }

        public async ValueTask<TokenContainer?> SetAccessTokenAsync(JwtSecurityToken token)
        {
            var container = new TokenContainer(token);
            await this.SetAccessTokenAsync(container);
            return container;
        }

        public ValueTask SetAccessTokenAsync(TokenContainer token)
        {
            this.AccessToken = token;
            return ValueTask.CompletedTask;
        }

        public async ValueTask<TokenContainer?> SetIdentityTokenAsync(string token)
        {
            var jwt = new JwtSecurityToken(token);
            return await this.SetIdentityTokenAsync(jwt);
        }

        public async ValueTask<TokenContainer?> SetIdentityTokenAsync(JwtSecurityToken token)
        {
            var container = new TokenContainer(token);
            await this.SetIdentityTokenAsync(container);
            return container;
        }

        public ValueTask SetIdentityTokenAsync(TokenContainer token)
        {
            this.IdentityToken = token;
            return ValueTask.CompletedTask;
        }

        public async ValueTask<TokenContainer?> SetRefreshTokenAsync(string token)
        {
            var container = new TokenContainer(token);
            await this.SetRefreshTokenAsync(container);
            return container;
        }

        public ValueTask SetRefreshTokenAsync(TokenContainer token)
        {
            this.RefreshToken = token;
            return ValueTask.CompletedTask;
        }
    }
}
