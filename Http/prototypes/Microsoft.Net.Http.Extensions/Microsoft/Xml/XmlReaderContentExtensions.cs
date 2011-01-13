// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Xml
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Xml;

    using Microsoft.Internal;

    public static class XmlReaderContentExtensions
    {
        public static XmlReader ReadAsXmlReader(this HttpContent content)
        {
            var settings = new XmlReaderSettings()
                {
                    CloseInput = true,
                    ConformanceLevel = ConformanceLevel.Auto,
                    MaxCharactersInDocument = 0,
                    IgnoreWhitespace = true,
                    IgnoreProcessingInstructions = true,
                    ProhibitDtd = true,
                };

            return ReadAsXmlReader(content, settings);
        }

        public static XmlReader ReadAsXmlReader(this HttpContent content, XmlReaderSettings settings)
        {
            FX.ThrowIfNull(content, "content");
            FX.ThrowIfNull(content, "settings");

            return XmlReader.Create(content.ContentReadStream, settings);
        }
    }
}
