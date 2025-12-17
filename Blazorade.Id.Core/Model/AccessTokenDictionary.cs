using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Represents a set of access tokens that are grouped by the resource they are intended for and keyed by the
    /// identifier of that resource. The dictionary is produced by an implementation of the 
    /// <see cref="Services.ITokenService"/> service interface.
    /// </summary>
    public class AccessTokenDictionary : DictionaryBase<string, TokenContainer>
    {

    }
}
