using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public class GetTokenOptions
    {
        public string? LoginHint { get; set; }

        public string? DomainHint { get; set; }

        public Prompt? Prompt { get; set; }

        public IEnumerable<string>? Scopes { get; set; }
    }

}
