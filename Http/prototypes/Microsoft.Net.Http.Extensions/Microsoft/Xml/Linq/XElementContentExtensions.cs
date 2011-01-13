// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>


namespace Microsoft.Xml.Linq
{
    using System.IO;
    using System.Net.Http;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Internal;

    public static class XElementContentExtensions
    {
        public static HttpContent ToContent(this XElement element, SaveOptions options = SaveOptions.None)
        {
            FX.ThrowIfNull(element, "element");

            var stream = new MemoryStream();
            try
            {
                element.Save(stream, options);
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

        public static XElement ReadAsXElement(this HttpContent content)
        {
            FX.ThrowIfNull(content, "content");

            using (var reader = XmlReaderContentExtensions.ReadAsXmlReader(content))
            {
                var e = XElement.Load(reader);
                return e;
            }
        }
    }
}
