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
    public class ScopeSorter : IScopeSorter
    {
        /// <inheritdoc/>
        public ScopeDictionary SortScopes(IEnumerable<string> scopes)
        {
            var result = new ScopeDictionary();

            Action<string, string> addScope = (resource, scope) =>
            {
                if (!result.ContainsKey(resource))
                {
                    result[resource] = new List<Scope>();
                }
                result[resource].Add(new Scope(scope));
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
                    resource = "ms-graph";
                }

                addScope(resource, scope);
            }

            return result;
        }
    }
}
