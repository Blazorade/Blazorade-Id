using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// Provides a default implementation for sorting and grouping scopes.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class DefaultScopeSorter : IScopeSorter
    {
        /// <inheritdoc/>
        public ScopeGroup SortScopes(IEnumerable<string> scopes)
        {
            var result = new ScopeGroup();

            Action<string, string> addScope = (resource, scope) =>
            {
                if (!result.ContainsKey(resource))
                {
                    result[resource] = new List<string>();
                }
                result[resource].Add(scope);
            };

            foreach (var scope in scopes)
            {
                string resource, localScope;
                if(scope.Contains('/'))
                {
                    resource = scope.Substring(0, scope.LastIndexOf('/'));
                    localScope = scope.Substring(scope.LastIndexOf('/') + 1);
                }
                else
                {
                    resource = "ms-graph";
                    localScope = scope;
                }

                addScope(resource, localScope);
            }

            return result;
        }
    }
}
