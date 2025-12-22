using Blazorade.Id.Configuration;
using Blazorade.Id.Model;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// A builder that is used to build URIs to various endpoints.
    /// </summary>
    public class EndpointUriBuilder : BuilderBase<string>
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public EndpointUriBuilder(string endpointUri)
        {
            var uri = new Uri(endpointUri, UriKind.Absolute);
            this.EndpointUri = uri.ToString();
        }

        /// <summary>
        /// Creates an instance of the class and specifies the base URI to start building on.
        /// </summary>
        public EndpointUriBuilder(string endpointUri, ICodeChallengeService codeChallengeService)
        {
            var uri = new Uri(endpointUri, UriKind.Absolute);
            this.EndpointUri = uri.ToString();
            this.CodeChallengeService = codeChallengeService;
        }

        private string EndpointUri;
        private ICodeChallengeService? CodeChallengeService;



        /// <summary>
        /// Adds an authorization code to the URI.
        /// </summary>
        public EndpointUriBuilder WithAuthorizationCode(string? code)
        {
            if (code?.Length > 0)
            {
                this.RemoveParameterValue(RefreshTokenName);
                this.Parameters[CodeName] = code;
                this.Parameters[GrantTypeName] = GrantTypeValueCode;
            }

            return this;
        }

        /// <summary>
        /// Adds a client ID to the URI.
        /// </summary>
        public EndpointUriBuilder WithClientId(string clientId)
        {
            this.Parameters[ClientIdName] = clientId;
            return this;
        }

        /// <summary>
        /// Uses the given code verifier and produces a code challenge that is added to the URI.
        /// </summary>
        /// <remarks>
        /// The code verifier is the same that you need to specify when acquiring the token
        /// with a code generated from the authorization endpoint.
        /// </remarks>
        public EndpointUriBuilder WithCodeChallenge(string? codeVerifier)
        {
            if(null == this.CodeChallengeService)
            {
                throw new InvalidOperationException("Cannot create code challenge when no code challenge service has been provided.");
            }

            if (codeVerifier?.Length > 0)
            {
                var challenge = this.CodeChallengeService.CreateCodeChallenge(codeVerifier);
                this.Parameters[CodeChallengeName] = challenge.ChallengeValue;
                this.Parameters[CodeChallengeMethodName] = challenge.ChallengeMethod;
            }

            return this;
        }

        /// <summary>
        /// Adds an ID token hint to the URI.
        /// </summary>
        /// <param name="idTokenHint"></param>
        /// <returns></returns>
        public EndpointUriBuilder WithIdTokenHint(string? idTokenHint)
        {
            if(idTokenHint?.Length > 0)
            {
                this.Parameters[IdTokenHintName] = idTokenHint;
            }
            return this;
        }

        /// <summary>
        /// Adds a login hint to the URI.
        /// </summary>
        public EndpointUriBuilder WithLoginHint(string? loginHint)
        {
            if(loginHint?.Length > 0)
            {
                this.Parameters[LoginHintName] = loginHint;
            }

            return this;
        }

        /// <summary>
        /// Adds the given nonce to the URI.
        /// </summary>
        public EndpointUriBuilder WithNonce(string? nonce)
        {
            if(nonce?.Length > 0)
            {
                this.Parameters[NonceName] = nonce;
            }
            return this;
        }

        /// <summary>
        /// Adds a redirect URI to the URI.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the URI where the user is redirected back to your application after signing in.
        /// </para>
        /// <para>
        /// Note that this URI must match one of the redirect URIs you have configured in the
        /// authentication settings for your application.
        /// </para>
        /// </remarks>
        public EndpointUriBuilder WithRedirectUri(string? redirectUri)
        {
            if(redirectUri?.Length > 0)
            {
                this.Parameters[RedirectUriName] = redirectUri;
            }

            return this;
        }

        /// <summary>
        /// Adds a redirect URI to the URI.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the URI where the user is redirected back to your application after signing in.
        /// </para>
        /// <para>
        /// Note that this URI must match one of the redirect URIs you have configured in the
        /// authentication settings for your application.
        /// </para>
        /// </remarks>
        public EndpointUriBuilder WithRedirectUri(Uri? redirectUri)
        {
            return this.WithRedirectUri(redirectUri?.ToString());
        }

        /// <summary>
        /// Adds a refresh token to the URI.
        /// </summary>
        public EndpointUriBuilder WithRefreshToken(string? refreshToken)
        {
            if (refreshToken?.Length > 0)
            {
                this.RemoveParameterValue(CodeName);
                this.Parameters[RefreshTokenName] = refreshToken;
                this.Parameters[GrantTypeName] = GrantTypeValueRefreshToken;
            }
            return this;
        }

        /// <summary>
        /// Adds a post-logout redirect URI to the URI.
        /// </summary>
        /// <remarks>This is the URI where the user is redirected after logging out.</remarks>
        public EndpointUriBuilder WithPostLogoutRedirectUri(string? postLogoutRedirectUri)
        {
            if(postLogoutRedirectUri?.Length > 0)
            {
                this.Parameters[PostLogoutRedirectUriName] = postLogoutRedirectUri;
            }

            return this;
        }

        /// <summary>
        /// Adds a response type to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple response types by calling the method multiple times.
        /// </remarks>
        public EndpointUriBuilder WithResponseType(ResponseType? responseType)
        {
            return this.WithResponseType(responseType?.ToString()?.ToLower());
        }

        /// <summary>
        /// Adds a response type to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple response types by calling the method multiple times.
        /// </remarks>
        public EndpointUriBuilder WithResponseType(string? responseType)
        {
            if(responseType?.Length > 0)
            {
                this.AddParameterValue(ResponseTypeName, responseType);
            }
            return this;
        }

        /// <summary>
        /// Adds a response mode to the URI.
        /// </summary>
        public EndpointUriBuilder WithResponseMode(ResponseMode? responseMode)
        {
            return this.WithResponseMode(responseMode?.ToString()?.ToLower());
        }

        /// <summary>
        /// Adds a response mode to the URI.
        /// </summary>
        public EndpointUriBuilder WithResponseMode(string? responseMode)
        {
            if(responseMode?.Length > 0)
            {
                this.Parameters[ResponseModeName] = responseMode.ToLower();
            }
            return this;
        }

        /// <summary>
        /// Adds a scope to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple scopes in one go by separating them with a space. You can also add one scope at a time by calling this method multiple times.
        /// </remarks>
        public EndpointUriBuilder WithScope(string? scope)
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
        public EndpointUriBuilder WithState(string? state)
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
        public EndpointUriBuilder WithPrompt(Prompt? prompt)
        {
            return this.WithPrompt(prompt?.ToString()?.ToLower());
        }

        /// <summary>
        /// Adds a prompt parameter to the URI.
        /// </summary>
        /// <remarks>
        /// The prompt controls how the user will be prompted in a UI.
        /// </remarks>
        public EndpointUriBuilder WithPrompt(string? prompt)
        {
            if(prompt?.Length > 0)
            {
                this.Parameters[PromptName] = prompt;
            }
            return this;
        }

        /// <summary>
        /// If specified, the email-based discovery process is skipped on the sign-in
        /// page, and the user is taken directly to the home tenant for authentication.
        /// </summary>
        public EndpointUriBuilder WithDomainHint(string? domainHint)
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
                .Append(this.EndpointUri)
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
