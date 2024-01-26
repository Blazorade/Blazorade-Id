using Blazorade.Id.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class TokenRequestBuilder : BuilderBase<HttpRequestMessage>
    {
        public TokenRequestBuilder(string tokenEndpointUri, IHttpClientFactory? clientFactory = null)
        {
            this.TokenEndpointUri = tokenEndpointUri;
            this.Client = clientFactory?.CreateClient() ?? new HttpClient();
        }

        private readonly string TokenEndpointUri;
        private readonly HttpClient Client;

        public static async Task<TokenRequestBuilder> CreateAsync(AuthenticationOptions options, IHttpClientFactory? clientFactory = null)
        {
            var epService = new EndpointService(clientFactory);
            var uri = await epService.GetTokenEndpointAsync(options);
            if(uri?.Length > 0)
            {
                return new TokenRequestBuilder(uri, clientFactory);
            }

            throw new Exception("Unable to resolve token endpoint URI.");
        }

        public TokenRequestBuilder WithClientId(string clientId)
        {
            this.Parameters[ClientIdName] = clientId;
            return this;
        }

        public TokenRequestBuilder WithScope(string? scope)
        {
            if(scope?.Length > 0)
            {
                this.AddParameterValue(ScopeName, scope);
            }
            return this;
        }

        public TokenRequestBuilder WithAuthorizationCode(string? code)
        {
            if(code?.Length > 0)
            {
                this.RemoveParameterValue(RefreshTokenName);
                this.Parameters[CodeName] = code;
                this.Parameters[GrantTypeName] = GrantTypeValueCode;
            }
            return this;
        }

        public TokenRequestBuilder WithRefreshToken(string? refreshToken)
        {
            if(refreshToken?.Length > 0)
            {
                this.RemoveParameterValue(CodeName);
                this.Parameters[RefreshTokenName] = refreshToken;
                this.Parameters[GrantTypeName] = GrantTypeValueRefreshToken;
            }
            return this;
        }

        public TokenRequestBuilder WithRedirectUri(string? redirectUri)
        {
            if(redirectUri?.Length > 0)
            {
                this.Parameters[RedirectUriName] = redirectUri;
            }
            return this;
        }

        public TokenRequestBuilder WithCodeVerifier(string? codeVerifier)
        {
            if(codeVerifier?.Length > 0)
            {
                if(codeVerifier.Length < 43)
                {
                    throw new ArgumentException("The code verifier must be at least 43 characters long.");
                }
                this.Parameters[CodeVerifierName] = codeVerifier;
            }
            return this;
        }

        public TokenRequestBuilder WithClientSecret(string? clientSecret)
        {
            if(clientSecret?.Length > 0)
            {
                this.Parameters[ClientSecretName] = clientSecret;
            }
            return this;
        }


        public override HttpRequestMessage Build()
        {
            HttpRequestMessage request = null!;

            try
            {
                request = new HttpRequestMessage(HttpMethod.Post, this.TokenEndpointUri)
                {
                    Content = new FormUrlEncodedContent(this.Parameters)
                };

                if (this.Parameters.ContainsKey(RedirectUriName))
                {
                    request.Headers.Add("Origin", this.Parameters[RedirectUriName]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while building token request message.", ex);
            }

            return request;
        }
    }
}
