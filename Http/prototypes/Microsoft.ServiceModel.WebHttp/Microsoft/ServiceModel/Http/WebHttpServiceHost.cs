// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using Description;

    public class WebHttpServiceHost : ServiceHost
    {
        public WebHttpServiceHost(Type serviceType, params Uri[] baseAddresses)
            : this(serviceType, null, baseAddresses)
        {
        }

        public WebHttpServiceHost(Type serviceType, HostConfiguration configuration, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            if (baseAddresses == null)
            {
                throw new ArgumentNullException("baseAddresses");
            }

            ContractDescription contract = ContractDescription.GetContract(serviceType);
            Description.Behaviors.Remove<AspNetCompatibilityRequirementsAttribute>();
            this.Description.Behaviors.Add(new AspNetCompatibilityRequirementsAttribute { RequirementsMode = AspNetCompatibilityRequirementsMode.Required });

            foreach (Uri baseAddress in baseAddresses)
            {
                ServiceEndpoint endpoint = new ServiceEndpoint(contract, new HttpMessageBinding(), new EndpointAddress(baseAddress));

                endpoint.Behaviors.Add(new HttpEndpointBehavior(configuration));
                this.AddServiceEndpoint(endpoint);
            }
        }
    }
}
