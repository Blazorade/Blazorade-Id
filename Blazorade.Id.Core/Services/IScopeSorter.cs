using Blazorade.Id.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// The service interface for a scope sorter.
    /// </summary>
    /// <remarks>
    /// A scope sorter is responsible for sorting Oauth2/OpenID Connect scopes into groups that represent
    /// the target resource they are intended for. Each target resource that an application connects to
    /// must have its own access token.
    /// </remarks>
    public interface IScopeSorter
    {
        /// <summary>
        /// Responseible for sorting the provided scopes into groups representing the target resources they belong to.
        /// </summary>
        /// <param name="scopes">The scopes to sort.</param>
        /// <returns>
        /// Returns a dictionary where the identifier for the target resource is the key, and the list of scopes
        /// belonging to that resource is the value.
        /// </returns>
        /// <remarks>
        /// This method is intentionally not an async method. This encourages implementations not to connect to
        /// external data sources to assist in the sorting process. The sorting should be done based on static
        /// information only.
        /// </remarks>
        ScopeDictionary SortScopes(IEnumerable<string> scopes);
    }
}
