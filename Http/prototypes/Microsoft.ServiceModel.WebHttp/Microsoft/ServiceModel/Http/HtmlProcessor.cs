// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System.Collections.Generic;
    using System.ServiceModel.Description;

    public class HtmlProcessor : PlainTextProcessor
    {
        public HtmlProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
            : base(operation, mode)
        {
        }

        public override IEnumerable<string> SupportedMediaTypes
        {
            get
            {
                yield return "text/html";
            }
        }
    }
}