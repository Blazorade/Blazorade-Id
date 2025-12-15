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

    }
}
