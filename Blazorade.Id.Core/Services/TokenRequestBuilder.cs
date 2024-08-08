using Blazorade.Id.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A request builder that is used to build requests to a token endpoint.
    /// </summary>
    public class TokenRequestBuilder : BuilderBase<HttpRequestMessage>
    {
        /// <summary>
        /// Creates a new instance of the class and specifies the token endpoint to send the built request to.
        /// </summary>
        public TokenRequestBuilder(string tokenEndpointUri)
        {
            this.TokenEndpointUri = tokenEndpointUri;
        }

        private readonly string TokenEndpointUri;



        /// <summary>
        /// Sets the client ID of the application requesting the information.
        /// </summary>
        public TokenRequestBuilder WithClientId(string clientId)
        {
            this.Parameters[ClientIdName] = clientId;
            return this;
        }

        /// <summary>
        /// Adds a scope to the request.
        /// </summary>
        public TokenRequestBuilder WithScope(string? scope)
        {
            if(scope?.Length > 0)
            {
                this.AddParameterValue(ScopeName, scope);
            }
            return this;
        }

        /// <summary>
        /// Sets the authorization code to with the request.
        /// </summary>
        /// <remarks>
        /// You should use either this method or <see cref="WithRefreshToken(string?)"/>, but not both.
        /// </remarks>
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

        /// <summary>
        /// Sets the refresh token to send with the request.
        /// </summary>
        /// <remarks>
        /// You should use either this method or <see cref="WithAuthorizationCode(string?)"/>, but not both.
        /// </remarks>
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

        /// <summary>
        /// Sets the redirect URI to send with the request.
        /// </summary>
        /// <remarks>
        /// This is required both with authorization codes and refresh tokens, because the
        /// redirect URI is also set as <c>Origin</c> header with the request to simulate
        /// that the request comes from a browser at the same URI that the user was originally
        /// redirected to from the authorization endpoint.
        /// </remarks>
        public TokenRequestBuilder WithRedirectUri(string? redirectUri)
        {
            if(redirectUri?.Length > 0)
            {
                this.Parameters[RedirectUriName] = redirectUri;
            }
            return this;
        }

        /// <summary>
        /// Sets the redirect URI to send with the request.
        /// </summary>
        /// <remarks>
        ///  This information is required if you use an authorization code.
        /// </remarks>
        public TokenRequestBuilder WithRedirectUri(Uri? redirectUri)
        {
            return this.WithRedirectUri(redirectUri?.ToString());
        }

        /// <summary>
        /// Sets the code verifier to send with the request.
        /// </summary>
        /// <remarks>
        /// This code verifier must be the same that was used to create a code challenge
        /// when sending the user to the authorization endpoint. Code challenges
        /// are created using the <see cref="CodeChallengeService"/>.
        /// </remarks>
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



        /// <summary>
        /// Builds the request using the information provided with the builder.
        /// </summary>
        /// <exception cref="Exception">
        /// The exception that is thrown if an exception occurs during building the request.
        /// </exception>
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
