using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A service that performs serialization services.
    /// </summary>
    public class SerializationService
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public SerializationService(IOptions<JsonSerializerOptions> options)
        {
            this.Options = options.Value;
        }

        private readonly JsonSerializerOptions Options;


        /// <summary>
        /// Takes the Base64 encoded string, decodes it and assumes that is a JSON string that is deserialized to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the value to.</typeparam>
        /// <param name="base64Json">A Base64 encoded JSON string to deserialize.</param>
        public T? DeserializeBase64String<T>(string base64Json) where T : class
        {
            var json = Base64UrlEncoder.Decode(base64Json);
            return JsonSerializer.Deserialize<T>(json, options: this.Options);
        }

        /// <summary>
        /// Takes the given object, serializes it to a JSON string and returns that string encoded as a Base64 encoded string.
        /// </summary>
        /// <param name="obj">The object to serialize and encode.</param>
        public string SerializeToBase64String(object obj)
        {
            var json = JsonSerializer.Serialize(obj, options: this.Options);
            return Base64UrlEncoder.Encode(json);
        }
    }
}
