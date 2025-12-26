using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Produces <see cref="HttpRequestMessage"/> instances that are configured with the appropriate bearer token
    /// matching the specified scopes.
    /// </summary>
    public class HttpRequestFactory : IHttpRequestFactory
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public HttpRequestFactory(ITokenService tokenService, IScopeAnalyzer scopeAnalyzer)
        {
            this.TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.ScopeAnalyzer = scopeAnalyzer ?? throw new ArgumentNullException(nameof(scopeAnalyzer));
        }

        private readonly ITokenService TokenService;
        private readonly IScopeAnalyzer ScopeAnalyzer;
        /// <inheritdoc/>
        public async Task<HttpRequestMessage?> CreateRequestAsync(string requestUri, HttpMethod method, IEnumerable<string> scopes, CancellationToken cancellationToken = default)
        {
            JwtSecurityToken? token = null;
            HttpRequestMessage? request = null;
            var analyzed = await this.ScopeAnalyzer.AnalyzeScopesAsync(scopes, cancellationToken);

            if(analyzed.Count > 0)
            {
                var tokens = await this.TokenService.GetAccessTokensAsync(new Model.GetTokenOptions { Scopes = analyzed.First().Value.Values() }, cancellationToken);
                token = tokens.Count > 0 ? tokens.First().Value : null;
            }

            if (null != token)
            {
                request = new HttpRequestMessage(method, requestUri);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.RawData);
            }

            return request;
        }
    }
}
