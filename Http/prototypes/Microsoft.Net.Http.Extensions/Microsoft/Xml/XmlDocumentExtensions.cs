// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Xml
{
    using System.IO;
    using System.Net.Http;
    using System.Xml;

    public static class XmlDocumentExtensions
    {
        public static HttpContent ToContent(this XmlDocument document)
        {
            var stream = new MemoryStream();
            try
            {
                document.Save(stream);
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

        public static XmlDocument ReadAsXmlDocument(this HttpContent content)
        {
            var document = new XmlDocument();
            document.Load(content.ContentReadStream);
            return document;
        }
    }
}
