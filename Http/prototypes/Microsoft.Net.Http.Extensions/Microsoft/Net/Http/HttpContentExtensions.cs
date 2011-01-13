// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>



namespace Microsoft.Net.Http
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.Internal;
    using Microsoft.Runtime.Serialization.Json;
    using Microsoft.Xml.Serialization;

    public static class HttpContentExtensions
    {
        public static T ReadAsObject<T>(this HttpContent content, params IContentFormatter[] formatters)
        {
            FX.ThrowIfNull(content, "content");

            var contentType = content.Headers.ContentType.MediaType;
            var formatter = formatters.FirstOrDefault(f => f.SupportedMediaTypes.Any(m => m.MediaType.Equals(contentType, StringComparison.OrdinalIgnoreCase)));

            if (formatter == null)
            {
                if (contentType.Equals("application/xml", StringComparison.OrdinalIgnoreCase) || contentType.Equals("text/xml", StringComparison.OrdinalIgnoreCase))
                {
                    return content.ReadAsXmlSerializable<T>();
                }
                else if (contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    return content.ReadAsJsonDataContract<T>();
                }

                throw new ArgumentOutOfRangeException("content", string.Format(CultureInfo.InvariantCulture, "No formatter is present to handle content type:{0}", content.Headers.ContentType.MediaType));
            }

            return (T)formatter.ReadFromStream(content.ContentReadStream);
        }

        public static HttpContent ToContent(this object instance, IContentFormatter formatter)
        {
            var stream = new MemoryStream();
            try
            {
                formatter.WriteToStream(instance, stream);
                stream.Position = 0;
                return new StreamContent(stream);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }
    }
}
