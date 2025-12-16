using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents a grouped set of scopes produced by an implementation of the 
    /// <see cref="Services.IScopeSorter"/> service interface.
    /// </summary>
    public class ScopeGroup : DictionaryBase<string, IList<string>>
    {

    }
}
