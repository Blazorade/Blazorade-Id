using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Core.Model
{
    public class RefreshableTokenSet : TokenSet
    {

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

    }
}
