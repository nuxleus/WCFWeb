// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel.Activation;

    using Microsoft.ServiceModel.Description;

    public interface IConfigurableServiceHostFactory
    {
        HttpHostConfiguration Configuration { get; set; }
    }

    public class WebHttpServiceHostFactory : ServiceHostFactory , IConfigurableServiceHostFactory
    {
        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebHttpServiceHost(serviceType, this.Configuration, baseAddresses);
        }

        public HttpHostConfiguration Configuration { get; set; }
    }
}
