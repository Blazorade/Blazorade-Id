using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blazorade.Id.Core.Services
{

    /// <summary>
    /// Base class for builder implementations.
    /// </summary>
    /// <typeparam name="TResult">The type of result that the builder produces.</typeparam>
    public abstract class BuilderBase<TResult> : BuilderBase
    {

        public abstract TResult Build();

    }

    public abstract class BuilderBase
    {
        protected const string ClientIdName = "client_id";
        protected const string RedirectUriName = "redirect_uri";
        protected const string PostLogoutRedirectUriName = "post_logout_redirect_uri";
        protected const string CodeName = "code";
        protected const string CodeVerifierName = "code_verifier";
        protected const string CodeChallengeName = "code_challenge";
        protected const string CodeChallengeMethodName = "code_challenge_method";
        protected const string RefreshTokenName = "refresh_token";
        protected const string LoginHintName = "login_hint";
        protected const string DomainHintName = "domain_hint";
        protected const string ResponseTypeName = "response_type";
        protected const string ResponseModeName = "response_mode";
        protected const string ScopeName = "scope";
        protected const string StateName = "state";
        protected const string PromptName = "prompt";
        protected const string GrantTypeName = "grant_type";

        protected const string CodeChallengeMethodValueS256 = "S256";
        protected const string GrantTypeValueCode = "authorization_code";
        protected const string GrantTypeValueRefreshToken = "refresh_token";

        protected Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

        protected void AddParameterValue(string key, string value)
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

        protected void RemoveParameterValue(string key)
        {
            if (this.Parameters.ContainsKey(key))
            {
                this.Parameters.Remove(key);
            }
        }
    }
}
