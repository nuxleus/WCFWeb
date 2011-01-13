namespace Microsoft.ServiceModel.Description
{
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using Microsoft.ServiceModel.Http;

    public interface IProcessorProvider
    {
        void RegisterRequestProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode);

        void RegisterResponseProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode);
    }
}