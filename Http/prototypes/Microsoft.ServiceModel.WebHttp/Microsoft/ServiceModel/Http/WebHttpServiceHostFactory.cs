// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel.Activation;

    using Microsoft.ServiceModel.Description;

    public class WebHttpServiceHostFactory : ServiceHostFactory
    {
        private HttpHostConfiguration configuration;

        [SuppressMessage("Microsoft.Design", "CA1026", Justification = "Uses optional params")]
        public WebHttpServiceHostFactory(HttpHostConfiguration configuration = null)
        {
            this.configuration = configuration;
        }

        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebHttpServiceHost(serviceType, this.configuration, baseAddresses);
        }
    }
}
