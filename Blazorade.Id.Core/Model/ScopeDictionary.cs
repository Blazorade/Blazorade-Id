using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents scopes that are grouped by the resource they are inteded for and keyed by the 
    /// identifier of that resource. The dictionary is produced by an implementation of the 
    /// <see cref="Services.IScopeSorter"/> service interface.
    /// </summary>
    public class ScopeDictionary : DictionaryBase<string, ScopeList>
    {

    }
}
