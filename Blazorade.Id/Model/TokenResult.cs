using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Model
{
    public class TokenResult
    {

        public string Token { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime Expires { get; set; }

    }
}
