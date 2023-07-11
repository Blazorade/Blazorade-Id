using Blazorade.Authentication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazorade.Authentication.Configuration
{
    public class EndpointUriBuilder : IAuthorizationEndpointUriBuilder, ITokenEndpointUriBuilder
    {
        private EndpointUriBuilder(string baseUri)
        {
            this.BaseUri = baseUri;
        }



        public static async Task<IAuthorizationEndpointUriBuilder> CreateAuthorizationEndpointUriBuilderAsync(BlazoradeAuthenticationOptions options, IHttpClientFactory? clientFactory)
        {
            var svc = new EndpointService(clientFactory);
            var uri = await svc.GetAuthorizationEndpointAsync(options) ?? throw new NullReferenceException("Could not resolve URI for authorization endpoint.");
            return new EndpointUriBuilder(uri);
        }

        public static async Task<ITokenEndpointUriBuilder> CreateTokenEndpointUriBuilderAsync(BlazoradeAuthenticationOptions options, IHttpClientFactory? clientFactory)
        {
            var svc = new EndpointService(clientFactory);
            var uri = await svc.GetTokenEndpointAsync(options) ?? throw new NullReferenceException("Could not resolve URI for token endpoint.");
            return new EndpointUriBuilder(uri);
        }

        private string BaseUri;
        private Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public string Build()
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



        #region IAuthorizationEndpointUriBuilder Members

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithClientId(string clientId)
        {
            this.Parameters["client_id"] = clientId;
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithCodeVerifier(string codeVerifier)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
            var codeChallenge = Convert.ToBase64String(hash);
            codeChallenge = Regex.Replace(codeChallenge, "\\+", "-");
            codeChallenge = Regex.Replace(codeChallenge, "\\/", "_");
            codeChallenge = Regex.Replace(codeChallenge, "=+$", "");
            this.Parameters["code_challenge"] = codeChallenge;
            this.Parameters["code_challenge_method"] = "S256";
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithLoginHint(string loginHint)
        {
            this.Parameters["login_hint"] = loginHint;
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithRedirectUri(string redirectUri)
        {
            this.Parameters["redirect_uri"] = redirectUri;
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithResponseType(ResponseType responseType)
        {
            this.AddParameterValue("response_type", responseType.ToString().ToLower());
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithScope(string scope)
        {
            this.AddParameterValue("scope", scope);
            return this;
        }

        IAuthorizationEndpointUriBuilder IAuthorizationEndpointUriBuilder.WithState(string state)
        {
            this.Parameters["state"] = state;
            return this;
        }

        #endregion

        #region ITokenEndpointUriBuilder Members

        ITokenEndpointUriBuilder ITokenEndpointUriBuilder.WithClientId(string clientId)
        {
            this.Parameters["client_id"] = clientId;
            return this;
        }

        ITokenEndpointUriBuilder ITokenEndpointUriBuilder.WithScope(string scope)
        {
            this.AddParameterValue("scope", scope);
            return this;
        }

        ITokenEndpointUriBuilder ITokenEndpointUriBuilder.WithCode(string code)
        {
            this.Parameters["code"] = code;
            return this;
        }

        ITokenEndpointUriBuilder ITokenEndpointUriBuilder.WithRedirectUri(string redirectUri)
        {
            this.Parameters["redirect_uri"] = redirectUri;
            return this;
        }

        ITokenEndpointUriBuilder ITokenEndpointUriBuilder.WithGrantType(GrantType grantType)
        {
            this.Parameters["grant_type"] = grantType.ToString().ToLower();
            return this;
        }

        ITokenEndpointUriBuilder ITokenEndpointUriBuilder.WithCodeVerifier(string codeVerifier)
        {
            this.Parameters["code_verifier"] = codeVerifier;
            return this;
        }

        #endregion



        private void AddParameterValue(string key, string value)
        {
            if (this.Parameters.ContainsKey(key))
            {
                var values = new List<string>(this.Parameters[key].Split(' '));
                values.Add(value);
                this.Parameters[key] = string.Join(' ', from x in values group x by x into y select y.Key);
            }
            else
            {
                this.Parameters[key] = value;
            }
        }

    }

    /// <summary>
    /// Defines the members for all endpoint URI builders.
    /// </summary>
    public interface IEndpointUriBuilder
    {
        /// <summary>
        /// When implemented in a class, builds the endpoint URI.
        /// </summary>
        string Build();
    }

    /// <summary>
    /// Defines members for a authorization endpoint URI builder.
    /// </summary>
    public interface IAuthorizationEndpointUriBuilder : IEndpointUriBuilder
    {
        /// <summary>
        /// Adds a client ID to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithClientId(string clientId);

        /// <summary>
        /// Adds a redirect URI to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithRedirectUri(string redirectUri);

        /// <summary>
        /// Adds a scope to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple scopes in one go by separating them with a space. You can also add one scope at a time by calling this method multiple times.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithScope(string scope);

        /// <summary>
        /// Adds a response type to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple response types by calling the method multiple times.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithResponseType(ResponseType responseType);

        /// <summary>
        /// Uses the given code verifier and produces a code challenge that is added to the URI.
        /// </summary>
        /// <remarks>
        /// The code verifier is the same that you need to specify when acquiring the token with a code generated from the authorization endpoint.
        /// </remarks>
        IAuthorizationEndpointUriBuilder WithCodeVerifier(string codeVerifier);

        /// <summary>
        /// Adds a state to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithState(string state);

        /// <summary>
        /// Adds a login hint to the URI.
        /// </summary>
        IAuthorizationEndpointUriBuilder WithLoginHint(string loginHint);
    }

    /// <summary>
    /// Defines members for a token endpoint URI builder.
    /// </summary>
    public interface ITokenEndpointUriBuilder : IEndpointUriBuilder
    {
        /// <summary>
        /// Adds a client ID to the URI.
        /// </summary>
        ITokenEndpointUriBuilder WithClientId(string clientId);

        /// <summary>
        /// Adds a scope to the URI.
        /// </summary>
        /// <remarks>
        /// You can add multiple scopes in one go by separating them with a space. You can also add one scope at a time by calling this method multiple times.
        /// </remarks>
        ITokenEndpointUriBuilder WithScope(string scope);

        /// <summary>
        /// Adds a code to the URI.
        /// </summary>
        ITokenEndpointUriBuilder WithCode(string code);

        /// <summary>
        /// Adds a redirect URI to the URI.
        /// </summary>
        ITokenEndpointUriBuilder WithRedirectUri(string redirectUri);

        /// <summary>
        /// Adds a grant type to the URI.
        /// </summary>
        ITokenEndpointUriBuilder WithGrantType(GrantType grantType);

        /// <summary>
        /// Adds a code verifier to the URI.
        /// </summary>
        ITokenEndpointUriBuilder WithCodeVerifier(string codeVerifier);
    }
}
