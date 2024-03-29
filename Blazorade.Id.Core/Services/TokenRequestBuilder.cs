﻿using Blazorade.Id.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class TokenRequestBuilder : BuilderBase<HttpRequestMessage>
    {
        public TokenRequestBuilder(string tokenEndpointUri)
        {
            this.TokenEndpointUri = tokenEndpointUri;
        }

        private readonly string TokenEndpointUri;



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

        public TokenRequestBuilder WithRedirectUri(Uri? redirectUri)
        {
            return this.WithRedirectUri(redirectUri?.ToString());
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
