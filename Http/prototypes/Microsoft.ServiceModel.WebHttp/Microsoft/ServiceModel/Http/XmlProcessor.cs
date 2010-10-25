// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.Web;
    using System.Xml.Serialization;

    using Microsoft.Http;

    public class XmlProcessor : MediaTypeProcessor
    {
        public XmlProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, mode)
        {
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get
            {
                return new List<string> { "text/xml", "application/xml" };
            }
        }

        public override void WriteToStream(object instance, System.IO.Stream stream, HttpRequestMessage request)
        {
            var serializer = new XmlSerializer(Parameter.ParameterType);
            serializer.Serialize(stream, instance);
        }

        public override object ReadFromStream(System.IO.Stream stream, HttpRequestMessage request)
        {
            var serializer = new XmlSerializer(Parameter.ParameterType);
            return serializer.Deserialize(stream);
        }
    }
}
