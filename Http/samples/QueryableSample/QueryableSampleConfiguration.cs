// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace QueryableSample
{
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ServiceModel.Description;
    using Microsoft.ServiceModel.Http;

    public class QueryableSampleConfiguration : HttpHostConfiguration, IProcessorProvider
    {
        public QueryableSampleConfiguration()
        {
            this.SetProcessorProvider(this);
        }

        public void RegisterRequestProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
        {
        }

        public void RegisterResponseProcessorsForOperation(System.ServiceModel.Description.HttpOperationDescription operation, IList<System.ServiceModel.Dispatcher.Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new JsonProcessor(operation, mode));
        }
    }
}