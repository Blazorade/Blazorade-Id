using Blazorade.Id.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    /// <summary>
    /// Provides a default implementation for sorting and grouping scopes.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class ScopeSorter : IScopeSorter
    {
        /// <summary>
        /// The default resource identifier for scopes associated with Microsoft Graph.
        /// </summary>
        public string MicrosoftGraphResourceId = "https://graph.microsoft.com";

        /// <inheritdoc/>
        public ScopeDictionary SortScopes(IEnumerable<string> scopes)
        {
            var result = new ScopeDictionary();

            Action<string, string> addScope = (resource, scope) =>
            {
                if (!result.ContainsKey(resource))
                {
                    result[resource] = new ScopeList();
                }
                result[resource].Add(scope);
            };

            foreach (var scope in scopes)
            {
                string resource;
                if(scope.Contains('/'))
                {
                    resource = scope.Substring(0, scope.LastIndexOf('/'));
                }
                else
                {
                    resource = MicrosoftGraphResourceId;
                }

                addScope(resource, scope);
            }

            return result;
        }
    }
}
