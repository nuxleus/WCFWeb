// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Json
{
    using System.IO;
    using System.Json;
    using System.Net.Http;
    using Microsoft.Internal;

    public static class JsonValueContentExtensions
    {
        public static HttpContent ToContent(this JsonValue json)
        {
            FX.ThrowIfNull(json, "json");

            var stream = new MemoryStream();
            try
            {
                json.Save(stream);
                stream.Position = 0;
                var content = new StreamContent(stream);
                return content;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        public static JsonValue ReadAsJsonValue(this HttpContent content)
        {
            FX.ThrowIfNull(content, "content");

            return JsonValue.Load(content.ContentReadStream);
        }
    }
}
