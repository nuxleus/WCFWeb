// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using Description;

    public class WebHttpServiceHost : ServiceHost
    {
        private readonly HttpHostConfiguration configuration;

        private readonly Type serviceType;

        public WebHttpServiceHost(object singletonInstance, params Uri[] baseAddresses)
            :this(singletonInstance, null, baseAddresses)
        {
        }

        public WebHttpServiceHost(object singletonInstance, HttpHostConfiguration configuration, params Uri[] baseAddresses)
            : base(singletonInstance, baseAddresses)
        {
            this.configuration = configuration;
            if (singletonInstance == null)
            {
                throw new ArgumentNullException("singletonInstance");
            }

            if (baseAddresses == null)
            {
                throw new ArgumentNullException("baseAddresses");
            }

            serviceType = singletonInstance.GetType();
            CreateEndpoints(baseAddresses);
        }


        public WebHttpServiceHost(Type serviceType, params Uri[] baseAddresses)
            : this(serviceType, null, baseAddresses)
        {
        }

        public WebHttpServiceHost(Type serviceType, HttpHostConfiguration configuration, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            this.configuration = configuration;
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            if (baseAddresses == null)
            {
                throw new ArgumentNullException("baseAddresses");
            }
            this.serviceType = serviceType;
            CreateEndpoints(baseAddresses);
        }

        private void CreateEndpoints(Uri[] baseAddresses)
        {
            //only create endpoints if there is not a factory for the ServiceDescription
            if (this.configuration == null || this.configuration.ServiceDescriptionFactory == null)
            {
                ContractDescription contract = ContractDescription.GetContract(this.serviceType);
                Description.Behaviors.Remove<AspNetCompatibilityRequirementsAttribute>();
                this.Description.Behaviors.Add(
                    new AspNetCompatibilityRequirementsAttribute
                        { RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed });

                foreach (Uri baseAddress in baseAddresses)
                {
                    ServiceEndpoint endpoint = new ServiceEndpoint(
                        contract, new HttpMessageBinding(), new EndpointAddress(baseAddress));

                    endpoint.Behaviors.Add(new HttpEndpointBehavior(this.configuration));
                    foreach (var configureEndpoint in configuration.EndpointConfiguration)
                    {
                        configureEndpoint.Configure(endpoint);
                    }
                    this.AddServiceEndpoint(endpoint);
                }
            }
        }

        protected override ServiceDescription CreateDescription(out System.Collections.Generic.IDictionary<string, ContractDescription> implementedContracts)
        {
            ServiceDescription description; 

            if (this.configuration != null && this.configuration.ServiceDescriptionFactory != null)
            {
                implementedContracts = new Dictionary<string, ContractDescription>();
                description =  this.configuration.ServiceDescriptionFactory.CreateDescription(this.serviceType, implementedContracts, this);
            }
            else
            {
                description = base.CreateDescription(out implementedContracts);
            }

            if (this.configuration != null)
            {
                ConfigureServiceDescription(description);
            }

            return description;
        }

        private void ConfigureServiceDescription(ServiceDescription description)
        {
            foreach(var serviceConfiguration in this.configuration.ServiceConfiguration)
            {
                serviceConfiguration.Configure(description);
            }

            foreach(var endpoint in description.Endpoints)
            {
                foreach(var endpointConfiguration in this.configuration.EndpointConfiguration)
                {
                    endpointConfiguration.Configure(endpoint);
                }

                foreach(var contractConfiguration in this.configuration.ContractConfiguration)
                {
                    contractConfiguration.Configure(endpoint.Contract);
                }

                foreach(var operation in endpoint.Contract.Operations)
                {
                    foreach(var operationConfiguration in this.configuration.OperationConfiguration)
                    {
                        operationConfiguration.Configure(operation);
                    }
                }
            }
        }

        protected override void OnOpening()
        {
            if (this.Description == null)
            {
                return;
            }

            // disable other things that listen for GET at base address and may conflict with auto-endpoints 
            ServiceDebugBehavior sdb = this.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb != null)
            {
                sdb.HttpHelpPageEnabled = false;
                sdb.HttpsHelpPageEnabled = false;
            }
            ServiceMetadataBehavior smb = this.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb != null)
            {
                smb.HttpGetEnabled = false;
                smb.HttpsGetEnabled = false;
            }

            base.OnOpening();
        }
    }
}
