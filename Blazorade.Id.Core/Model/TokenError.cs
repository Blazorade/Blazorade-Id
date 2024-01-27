using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blazorade.Id.Core.Model
{
    public class TokenError
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; } = string.Empty;

        [JsonPropertyName("error_codes")]
        public List<int> ErrorCodes { get; set; } = new List<int>();

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("trace_id")]
        public string TraceId { get; set; } = string.Empty;

        [JsonPropertyName("correlation_id")]
        public string CorrelationId {  get; set; } = string.Empty;

        [JsonPropertyName("error_uri")]
        public string ErrorUri {  get; set; } = string.Empty;
    }
}
