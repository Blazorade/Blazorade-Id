using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class AuthorizationEndpointUriBuilder : BuilderBase<string>
    {
        public AuthorizationEndpointUriBuilder(string authorizationEndpoint)
        {
            this.AuthorizationEndpoint = authorizationEndpoint;
        }



        public static async Task<AuthorizationEndpointUriBuilder> CreateAuthorizationEndpointUriBuilderAsync(BlazoradeAuthenticationOptions options, IHttpClientFactory? clientFactory = null)
        {
            var svc = new EndpointService(clientFactory);
            var uri = await svc.GetAuthorizationEndpointAsync(options) ?? throw new NullReferenceException("Could not resolve URI for authorization endpoint.");
            return new AuthorizationEndpointUriBuilder(uri);
        }

        private string AuthorizationEndpoint;


        /// <summary>
        /// Adds a client ID to the URI.
        /// </summary>
        public AuthorizationEndpointUriBuilder WithClientId(string clientId)
        {
            this.Parameters[ClientIdName] = clientId;
            return this;
        }

        /// <summary>
        /// Uses the given code verifier and produces a code challenge that is added to the URI.
        /// </summary>
        /// <remarks>
        /// The code verifier is the same that you need to specify when acquiring the token with a code generated from the authorization endpoint.
        /// </remarks>
        public AuthorizationEndpointUriBuilder WithCodeVerifier(string? codeVerifier)
        {
            if(codeVerifier?.Length > 0)
            {
                if (codeVerifier.Length < 43)
                {
                    throw new ArgumentException("The code verifier must be at least 43 characters long.");
                }

                var sha = SHA256.Create();
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                var codeChallenge = Convert.ToBase64String(hash);
                codeChallenge = Regex.Replace(codeChallenge, "\\+", "-");
                codeChallenge = Regex.Replace(codeChallenge, "\\/", "_");
                codeChallenge = Regex.Replace(codeChallenge, "=+$", "");
                this.Parameters[CodeChallengeName] = codeChallenge;
                this.Parameters[CodeChallengeMethodName] = CodeChallengeMethodValueS256;
            }

            return this;
        }

        /// <summary>
        /// Adds a login hint to the URI.
        /// </summary>
        public AuthorizationEndpointUriBuilder WithLoginHint(string? loginHint)
        {
            if(loginHint?.Length > 0)
            {
                this.Parameters[LoginHintName] = loginHint;
            }

            return this;
        }

        /// <summary>
        /// Adds a redirect URI to the URI.
        /// </summary>
        public AuthorizationEndpointUriBuilder WithRedirectUri(string? redirectUri)
        {
            if(redirectUri?.Length > 0)
            {
                this.Parameters[redirectUri] = redirectUri;
            }

            return this;
        }

        /// <summary>
        /// Adds a response type to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple response types by calling the method multiple times.
        /// </remarks>
        public AuthorizationEndpointUriBuilder WithResponseType(ResponseType responseType)
        {
            this.AddParameterValue(ResponseTypeName, responseType.ToString().ToLower());
            return this;
        }

        /// <summary>
        /// Adds a response mode to the URI.
        /// </summary>
        /// <param name="responseMode"></param>
        /// <returns></returns>
        public AuthorizationEndpointUriBuilder WithResponseMode(ResponseMode responseMode)
        {
            this.Parameters[ResponseModeName] = responseMode.ToString().ToLower();
            return this;
        }

        /// <summary>
        /// Adds a scope to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple scopes in one go by separating them with a space. You can also add one scope at a time by calling this method multiple times.
        /// </remarks>
        public AuthorizationEndpointUriBuilder WithScope(string? scope)
        {
            if(scope?.Length > 0)
            {
                this.AddParameterValue(ScopeName, scope);
            }

            return this;
        }

        /// <summary>
        /// Adds a state to the URI.
        /// </summary>
        public AuthorizationEndpointUriBuilder WithState(string? state)
        {
            if(state?.Length > 0)
            {
                this.Parameters[StateName] = state;
            }

            return this;
        }

        /// <summary>
        /// Adds a prompt parameter to the URI.
        /// </summary>
        /// <remarks>
        /// The prompt controls how the user will be prompted in a UI.
        /// </remarks>
        /// <param name="prompt"></param>
        public AuthorizationEndpointUriBuilder WithPrompt(Prompt prompt)
        {
            return this.WithPrompt(prompt.ToString().ToLower());
        }

        /// <summary>
        /// Adds a prompt parameter to the URI.
        /// </summary>
        /// <remarks>
        /// The prompt controls how the user will be prompted in a UI.
        /// </remarks>
        public AuthorizationEndpointUriBuilder WithPrompt(string? prompt)
        {
            if(prompt?.Length > 0)
            {
                this.AddParameterValue(PromptName, prompt);
            }
            return this;
        }

        /// <summary>
        /// If specified, the email-based discovery process is skipped on the sign-in
        /// page, and the user is taken directly to the home tenant for authentication.
        /// </summary>
        public AuthorizationEndpointUriBuilder WithDomainHint(string? domainHint)
        {
            if(domainHint?.Length > 0)
            {
                this.Parameters[DomainHintName] = domainHint;
            }

            return this;
        }

        /// <summary>
        /// Builds the URI.
        /// </summary>
        public override string Build()
        {
            var sb = new StringBuilder()
                .Append(this.AuthorizationEndpoint)
                .Append("?");

            for (int i = 0; i < this.Parameters.Count; i++)
            {
                if (i > 0) sb.Append("&");

                var key = this.Parameters.Keys.ElementAt(i);
                sb
                    .Append(key)
                    .Append("=")
                    .Append(this.Parameters[key]);
            }

            return sb.ToString();
        }

    }

}
