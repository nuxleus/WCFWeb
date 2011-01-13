// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel.Description;

    using System.Net.Http;

    public class PlainTextProcessor : MediaTypeProcessor 
    {
        public PlainTextProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, mode)
        {
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get
            {
                yield return "text/plain";
            }
        }

        public override void WriteToStream(object instance, Stream stream, HttpRequestMessage request)
        {
            var output = (string)instance;
            var writer = new StreamWriter(stream);
            writer.Write(output);
        }

        public override object ReadFromStream(Stream stream, HttpRequestMessage request)
        {
            throw new InvalidOperationException();
        }
    }
}
