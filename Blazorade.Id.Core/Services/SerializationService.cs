using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Blazorade.Id.Core.Services
{
    public class SerializationService
    {
        public SerializationService(IOptionsFactory<JsonSerializerOptions> optionsFactory)
        {
            this.Options = optionsFactory.Create("");
        }

        private readonly JsonSerializerOptions Options;


        public T? DeserializeBase64String<T>(string base64Json) where T : class
        {
            var json = Base64UrlEncoder.Decode(base64Json);
            return JsonSerializer.Deserialize<T>(json, options: this.Options);
        }

        public string SerializeToBase64String(object obj)
        {
            var json = JsonSerializer.Serialize(obj, options: this.Options);
            return Base64UrlEncoder.Encode(json);
        }
    }
}
