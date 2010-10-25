// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Dispatcher;
    using Http;

    public static class ProcessorListExtensions
    {
        public static void ClearMediaTypeProcessors(this IList<Processor> processors)
        {
            if (processors == null)
            {
                throw new ArgumentNullException("processors");
            }

            var processorsToRemove = processors.OfType<MediaTypeProcessor>().ToList();
            foreach (var processor in processorsToRemove)
            {
                processors.Remove(processor);
            }
        }
    }
}
