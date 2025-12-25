using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Provides a default implementation for sorting, grouping and analyzing scopes.
    /// </summary>
    public class ScopeAnalyzer : IScopeAnalyzer
    {
        /// <summary>
        /// The default resource identifier for scopes associated with Microsoft Graph.
        /// </summary>
        public string MicrosoftGraphResourceId = "https://graph.microsoft.com";

        /// <inheritdoc/>
        public Task<ScopeDictionary> AnalyzeScopesAsync(IEnumerable<string> scopes, CancellationToken cancellationToken = default)
        {
            var result = new ScopeDictionary();

            Action<string, Scope> addScope = (resource, scope) =>
            {
                if (!result.ContainsKey(resource))
                {
                    result[resource] = new ScopeList();
                }
                result[resource].Add(scope);
            };

            foreach (var scope in scopes)
            {
                Scope scp;
                string resource;
                if(scope.Contains('/'))
                {
                    resource = scope.Substring(0, scope.LastIndexOf('/'));
                    scp = new Scope(scope);
                }
                else
                {
                    resource = MicrosoftGraphResourceId;
                    scp = new Scope(scope, scope.EndsWith(".All") ? ScopeClassification.Elevated : ScopeClassification.Default);
                }

                addScope(resource, scp);
            }

            return Task.FromResult(result);
        }
    }
}
