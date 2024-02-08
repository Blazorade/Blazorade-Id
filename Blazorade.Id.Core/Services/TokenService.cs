using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class TokenService
    {
        public TokenService(IHttpClientFactory httpFactory, StorageFacade storage, IOptionsFactory<AuthorityOptions> authOptions, EndpointService epService, IOptions<JsonSerializerOptions> jsonOptions)
        {
            this.HttpClientFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.StorageFacade = storage ?? throw new ArgumentNullException(nameof(storage));
            this.AuthOptions = authOptions ?? throw new ArgumentNullException(nameof(authOptions));
            this.EndpointService = epService ?? throw new ArgumentNullException(nameof(epService));
            this.JsonOptions = jsonOptions.Value ?? throw new ArgumentNullException(nameof(jsonOptions));

        }

        private readonly IHttpClientFactory HttpClientFactory;
        private readonly StorageFacade StorageFacade;
        private readonly IOptionsFactory<AuthorityOptions> AuthOptions;
        private readonly EndpointService EndpointService;
        private readonly JsonSerializerOptions JsonOptions;

        public async ValueTask<OperationResult<TokenSet>> ProcessAuthorizationCodeAsync(string code, string redirectUri, string? authorityKey = null)
        {
            TokenSet? tokenSet = null;
            OperationError? error = null;

            var uri = new Uri(redirectUri);
            if(!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("The given redirect URI must be an absolute URI, and it must match the redirect URI that was specified in the login request sent to the authorization endpoint.", nameof(redirectUri));
            }

            var codeVerifier = await this.StorageFacade.GetCodeVerifierAsync();
            var scope = await this.StorageFacade.GetScopeAsync();
            var options = this.AuthOptions.Create(authorityKey ?? "");

            var tokenRequestBuilder = await this.EndpointService.CreateTokenRequestBuilderAsync(options);
            var tokenRequest = tokenRequestBuilder
                .WithClientId(options.ClientId)
                .WithAuthorizationCode(code)
                .WithCodeVerifier(codeVerifier)
                .WithScope(scope)
                .WithRedirectUri(uri)
                .Build();

            var client = this.HttpClientFactory.CreateClient();

            try
            {
                var now = DateTime.UtcNow;
                using (var response = await client.SendAsync(tokenRequest))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        tokenSet = JsonSerializer.Deserialize<TokenSet>(content, options: this.JsonOptions);
                        if(null != tokenSet)
                        {
                            tokenSet.ExpiresAtUtc = now.AddSeconds(tokenSet.ExpiresIn);
                        }
                    }
                    else
                    {
                        error = new OperationError { Code = response.StatusCode.ToString(), Description = content };
                    };
                }
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message  };
            }

            if(tokenSet?.RefreshToken?.Length > 0)
            {
                await this.StorageFacade.SetRefreshTokenAsync(new TokenContainer(tokenSet.RefreshToken, tokenSet.ExpiresAtUtc));
            }

            if(tokenSet?.IdentityToken?.Length > 0)
            {
                await this.ProcessIdentityTokenAsync(tokenSet.IdentityToken, authorityKey);
            }

            if(tokenSet?.AccessToken?.Length > 0)
            {
                await this.ProcessAccessTokenAsync(tokenSet.AccessToken, authorityKey);
            }

            return new OperationResult<TokenSet>(tokenSet, error);
        }

        public async ValueTask<OperationResult<TokenContainer>> ProcessIdentityTokenAsync(string idToken, string? authorityKey = null)
        {
            JwtSecurityToken? token = null;
            var options = this.AuthOptions.Create(authorityKey ?? "");

            return new OperationResult<TokenContainer>(new TokenContainer { Token = null });
        }

        public async ValueTask<OperationResult<JwtSecurityToken>> ProcessAccessTokenAsync(string accessToken, string? authorityKey = null)
        {
            JwtSecurityToken? token = null;
            var options = this.AuthOptions.Create(authorityKey ?? "");

            return new OperationResult<JwtSecurityToken>(token);
        }
    }
}
