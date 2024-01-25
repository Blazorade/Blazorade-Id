using Blazorade.Id.Core.Configuration;
using Blazorade.Id.Core.Services;
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
    public class AuthorizationEndpointUriBuilder : BuilderBase<string>, IAuthorizationEndpointUriBuilder
    {
        private AuthorizationEndpointUriBuilder(string baseUri)
        {
            this.BaseUri = baseUri;
        }



        public static async Task<IAuthorizationEndpointUriBuilder> CreateAuthorizationEndpointUriBuilderAsync(BlazoradeAuthenticationOptions options, IHttpClientFactory? clientFactory = null)
        {
            var svc = new EndpointService(clientFactory);
            var uri = await svc.GetAuthorizationEndpointAsync(options) ?? throw new NullReferenceException("Could not resolve URI for authorization endpoint.");
            return new AuthorizationEndpointUriBuilder(uri);
        }

        private string BaseUri;

        public override string Build()
        {
            var sb = new StringBuilder()
                .Append(this.BaseUri)
                .Append("?");

            for(int i = 0; i < this.Parameters.Count; i++)
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




        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithClientId(string clientId)
        {
            this.Parameters["client_id"] = clientId;
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithCodeVerifier(string? codeVerifier)
        {
            if(codeVerifier?.Length > 0)
            {
                var sha = SHA256.Create();
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                var codeChallenge = Convert.ToBase64String(hash);
                codeChallenge = Regex.Replace(codeChallenge, "\\+", "-");
                codeChallenge = Regex.Replace(codeChallenge, "\\/", "_");
                codeChallenge = Regex.Replace(codeChallenge, "=+$", "");
                this.Parameters["code_challenge"] = codeChallenge;
                this.Parameters["code_challenge_method"] = "S256";
            }

            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithLoginHint(string? loginHint)
        {
            if(loginHint?.Length > 0)
            {
                this.Parameters["login_hint"] = loginHint;
            }

            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithRedirectUri(string? redirectUri)
        {
            if(redirectUri?.Length > 0)
            {
                this.Parameters["redirect_uri"] = redirectUri;
            }

            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithResponseType(ResponseType responseType)
        {
            this.AddParameterValue("response_type", responseType.ToString().ToLower());
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithResponseMode(ResponseMode responseMode)
        {
            this.Parameters["response_mode"] = responseMode.ToString().ToLower();
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithScope(string? scope)
        {
            if(scope?.Length > 0)
            {
                this.AddParameterValue("scope", scope);
            }

            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithState(string? state)
        {
            if(state?.Length > 0)
            {
                this.Parameters["state"] = state;
            }

            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithPrompt(Prompt prompt)
        {
            return ((IAuthorizationEndpointUriBuilder)this).WithPrompt(prompt.ToString().ToLower());
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithPrompt(string? prompt)
        {
            if(prompt?.Length > 0)
            {
                this.AddParameterValue("prompt", prompt);
            }
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithDomainHint(string? domainHint)
        {
            if(domainHint?.Length > 0)
            {
                this.Parameters["domain_hint"] = domainHint;
            }

            return this;
        }


        protected override void AddParameterValue(string key, string value)
        {
            if (this.Parameters.ContainsKey(key))
            {
                var values = new List<string>(this.Parameters[key].Split(' '));
                values.Add(value);
                base.AddParameterValue(key, string.Join(' ', from x in values group x by x into y select y.Key));
            }

            else
            {
                base.AddParameterValue(key, value);
            }
        }

    }

    /// <summary>
    /// Defines members for a authorization endpoint URI builder.
    /// </summary>
    public interface IAuthorizationEndpointUriBuilder
    {
        /// <summary>
        /// Adds a client ID to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithClientId(string clientId);

        /// <summary>
        /// Adds a redirect URI to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithRedirectUri(string? redirectUri);

        /// <summary>
        /// Adds a scope to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple scopes in one go by separating them with a space. You can also add one scope at a time by calling this method multiple times.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithScope(string? scope);

        /// <summary>
        /// Adds a response type to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple response types by calling the method multiple times.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithResponseType(ResponseType responseType);

        IAuthorizationEndpointUriBuilder WithResponseMode(ResponseMode responseMode);

        /// <summary>
        /// Uses the given code verifier and produces a code challenge that is added to the URI.
        /// </summary>
        /// <remarks>
        /// The code verifier is the same that you need to specify when acquiring the token with a code generated from the authorization endpoint.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithCodeVerifier(string? codeVerifier);

        /// <summary>
        /// Adds a state to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithState(string? state);

        /// <summary>
        /// Adds a login hint to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithLoginHint(string? loginHint);

        /// <summary>
        /// Adds a prompt parameter to the URI.
        /// </summary>
        /// <remarks>
        /// The prompt controls how the user will be prompted in a UI.
        /// </remarks>
        /// <param name="prompt"></param>
        IAuthorizationEndpointUriBuilder WithPrompt(Prompt prompt);

        /// <summary>
        /// Adds a prompt parameter to the URI.
        /// </summary>
        /// <remarks>
        /// The prompt controls how the user will be prompted in a UI.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithPrompt(string? prompt);

        /// <summary>
        /// If specified, the email-based discovery process is skipped on the sign-in
        /// page, and the user is taken directly to the home tenant for authentication.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithDomainHint(string? domainHint);

        /// <summary>
        /// When implemented in a class, builds the endpoint URI.
        /// </summary>
        string Build();
    }

}
