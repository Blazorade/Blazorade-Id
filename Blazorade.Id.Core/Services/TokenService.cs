using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

                await this.StorageFacade.RemoveScopeAsync();
                await this.StorageFacade.RemoveCodeVerifierAsync();
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message  };
            }

            if(tokenSet?.RefreshToken?.Length > 0)
            {
                // We don't set the expiration for the refresh token, because it does not expire at the same time with 
                // the other tokens.
                await this.StorageFacade.SetRefreshTokenAsync(new TokenContainer(tokenSet.RefreshToken, null));
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
            TokenContainer? container = null;
            OperationError? error = null;

            // aud = clientId

            try
            {
                token = new JwtSecurityToken(idToken);
                var expires = this.GetExpirationTimeUtc(token);
                if(expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                var nonce = await this.StorageFacade.GetNonceAsync();
                var tokenNonce = this.GetNonce(token);
                await this.StorageFacade.RemoveNonceAsync();

                if(tokenNonce?.Length > 0 && tokenNonce != nonce)
                {
                    // If the token has a nonce specified, but it does not match
                    // the one that was stored for the session, then we cannot
                    // accept the token.
                    throw new Exception("The token nonce does not match the requested nonce.");
                }

                var username = this.GetClaimValue(token, "preferred_username");
                await this.StorageFacade.SetUsernameAsync(username);

                container = new TokenContainer(idToken, expires);
                await this.StorageFacade.SetIdentityTokenAsync(container);
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message };
            }

            container = new TokenContainer(token: idToken, expires: null);
            return new OperationResult<TokenContainer>(value: container, error: error);
        }

        public async ValueTask<OperationResult<TokenContainer>> ProcessAccessTokenAsync(string accessToken, string? authorityKey = null)
        {
            JwtSecurityToken? token = null;
            TokenContainer? container = null;
            OperationError? error = null;

            // appid = clientId

            try
            {
                token = new JwtSecurityToken(accessToken);
                var expires = this.GetExpirationTimeUtc(token);
                if (expires < DateTime.UtcNow)
                {
                    throw new Exception("Token is expired.");
                }

                container = new TokenContainer(accessToken, expires);
                await this.StorageFacade.SetAccessTokenAsync(container);
            }
            catch (Exception ex)
            {
                error = new OperationError { Description = ex.Message };
            }

            container = new TokenContainer(token: accessToken, expires: null);
            return new OperationResult<TokenContainer>(value: container, error: error);
        }

        private DateTime? GetExpirationTimeUtc(JwtSecurityToken token)
        {
            var exp = this.GetClaimValue(token, "exp");
            if(long.TryParse(exp, out long l))
            {
                var expires = DateTimeOffset.FromUnixTimeSeconds(l).UtcDateTime;
                return expires;
            }

            return null;
        }

        private string? GetNonce(JwtSecurityToken token)
        {
            return this.GetClaimValue(token, "nonce");
        }

        private string? GetClaimValue(JwtSecurityToken token, string claimType)
        {
            return token.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value;
        }
    }
}
