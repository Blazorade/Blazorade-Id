using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Model
{
    /// <summary>
    /// Options for refreshing tokens.
    /// </summary>
    public class TokenRefreshOptions
    {
        /// <summary>
        /// The scopes the tokens should be refreshed for.
        /// </summary>
        public IEnumerable<string> Scopes { get; set; } = [];
    }
}
