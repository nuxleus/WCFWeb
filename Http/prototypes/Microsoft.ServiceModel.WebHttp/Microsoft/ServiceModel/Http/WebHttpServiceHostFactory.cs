// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel.Activation;

    public class WebHttpServiceHostFactory : ServiceHostFactory
    {
        private HostConfiguration processorFactory;

        [SuppressMessage("Microsoft.Design", "CA1026", Justification = "Uses optional params")]
        public WebHttpServiceHostFactory(HostConfiguration processorFactory = null)
        {
            this.processorFactory = processorFactory;
        }

        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebHttpServiceHost(serviceType, this.processorFactory, baseAddresses);
        }
    }
}
