// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Microsoft.ServiceModel.Description;

    public class HttpHostConfiguration
    {
        private readonly IList<IServiceConfiguration> serviceConfiguration = new List<IServiceConfiguration>();
        private readonly IList<IEndpointConfiguration> endpointConfiguration = new List<IEndpointConfiguration>();
        private readonly IList<IContractConfiguration> contractConfiguration = new List<IContractConfiguration>();
        private readonly IList<IOperationConfiguration> operationConfiguration = new List<IOperationConfiguration>();

        public HttpHostConfiguration()
        {
            AddImplementedConfiguration();
        }

        //this is a convenience method to support a more friendly programming model.
        private void AddImplementedConfiguration()
        {
            if (this is IServiceConfiguration)
            {
                this.AddServiceConfiguration((IServiceConfiguration)this);
            }

            if (this is IEndpointConfiguration)
            {
                this.AddEndpointConfiguration((IEndpointConfiguration)this);
            }

            if (this is IContractConfiguration)
            {
                this.AddContractConfiguration((IContractConfiguration)this);
            }

            if (this is IOperationConfiguration)
            {
                this.AddOperationConfiguration((IOperationConfiguration)this);
            }

            if (this is IInstanceFactory)
            {
                this.SetInstanceFactory((IInstanceFactory)this);
            }

            if (this is IServiceDescriptionFactory)
            {
                this.SetServiceDescriptionFactory((IServiceDescriptionFactory)this);
            }

            if (this is IProcessorProvider)
            {
                this.SetProcessorProvider((IProcessorProvider)this);
            }
        }

        public IEnumerable<IServiceConfiguration> ServiceConfiguration
        {
            get { return this.serviceConfiguration; }
        }

        public IEnumerable<IEndpointConfiguration> EndpointConfiguration
        {
            get { return this.endpointConfiguration; }
        }

        public IEnumerable<IContractConfiguration> ContractConfiguration
        {
            get { return this.contractConfiguration; }
        }

        public IEnumerable<IOperationConfiguration> OperationConfiguration
        {
            get { return this.operationConfiguration; }
        }

        public IServiceDescriptionFactory ServiceDescriptionFactory { get; private set; }

        public IInstanceFactory InstanceFactory { get; private set; }

        public IProcessorProvider ProcessorProvider { get; private set; }

        public HttpMessageChannel Channel { get; private set; }

        public HttpHostConfiguration SetChannel(HttpMessageChannel channel)
        {
            this.Channel = channel;
            return this;
        }

        public HttpHostConfiguration AddServiceConfiguration(IServiceConfiguration configureService)
        {
            this.serviceConfiguration.Add(configureService);
            return this;
        }

        public HttpHostConfiguration AddServiceConfiguration(Action<ServiceDescription> configureService)
        {
            this.serviceConfiguration.Add(new DelegateServiceConfiguration(configureService));
            return this;
        }
        
        public HttpHostConfiguration AddEndpointConfiguration(IEndpointConfiguration configureEndpoint)
        {
            this.endpointConfiguration.Add(configureEndpoint);
            return this;
        }

        public HttpHostConfiguration AddEndpointConfiguration(Action<ServiceEndpoint> configuredEndpoint)
        {
            this.endpointConfiguration.Add(new DelegateEndpointConfiguration(configuredEndpoint));
            return this;
        }

        public HttpHostConfiguration AddContractConfiguration(IContractConfiguration configureContract)
        {
            this.contractConfiguration.Add(configureContract);
            return this;
        }

        public HttpHostConfiguration AddContractConfiguration(Action<ContractDescription> configureContract)
        {
            this.contractConfiguration.Add(new DelegateContractConfiguration(configureContract));
            return this;
        }

        public HttpHostConfiguration AddOperationConfiguration(IOperationConfiguration configureOperation)
        {
            this.operationConfiguration.Add(configureOperation);
            return this;
        }

        public HttpHostConfiguration AddOperationConfiguration(Action<OperationDescription> configureOperation)
        {
            this.operationConfiguration.Add(new DelegateOperationConfiguration(configureOperation));
            return this;
        }

        public HttpHostConfiguration SetInstanceFactory(IInstanceFactory factory)
        {
            this.InstanceFactory = factory;
            return this;
        }

        public HttpHostConfiguration SetServiceDescriptionFactory(IServiceDescriptionFactory factory)
        {
            this.ServiceDescriptionFactory = factory;
            return this;
        }

        public HttpHostConfiguration SetServiceDescriptionFactory(Func<Type, IDictionary<string, ContractDescription>, ServiceHost, ServiceDescription> factory)
        {
            this.ServiceDescriptionFactory = new DelegateServiceDescriptionFactory(factory);
            return this;
        }


        public HttpHostConfiguration SetInstanceFactory(Func<Type, InstanceContext, Message, object> getInstance, Action<InstanceContext, object> releaseInstance)
        {
            this.InstanceFactory = new DelegateInstanceFactory(getInstance, releaseInstance);
            return this;
        }

        public HttpHostConfiguration SetProcessorProvider(IProcessorProvider provider)
        {
            this.ProcessorProvider = provider;
            return this;
        }

        public HttpHostConfiguration SetProcessorProvider(Action<HttpOperationDescription, IList<Processor>, MediaTypeProcessorMode> requestProcessors, Action<HttpOperationDescription, IList<Processor>, MediaTypeProcessorMode> responseProcessors)
        {
            this.ProcessorProvider = new DelegateProcessorProvider(requestProcessors, responseProcessors);
            return this;
        }

        class DelegateServiceConfiguration : IServiceConfiguration
        {
            private readonly Action<ServiceDescription> configureService;

            public DelegateServiceConfiguration(Action<ServiceDescription> configureService)
            {
                this.configureService = configureService;
            }

            public void Configure(ServiceDescription service)
            {
                this.configureService(service);
            }
        }

        class DelegateEndpointConfiguration : IEndpointConfiguration
        {
            private readonly Action<ServiceEndpoint> configureEndpoint;

            public DelegateEndpointConfiguration(Action<ServiceEndpoint> configureEndpoint)
            {
                this.configureEndpoint = configureEndpoint;
            }

            public void Configure(ServiceEndpoint endpoint)
            {
                this.configureEndpoint(endpoint);
            }
        }

        class DelegateContractConfiguration : IContractConfiguration
        {
            private readonly Action<ContractDescription> configureContract;

            public DelegateContractConfiguration(Action<ContractDescription> configureContract)
            {
                this.configureContract = configureContract;
            }

            public void Configure(ContractDescription contract)
            {
                this.configureContract(contract);
            }
        }

        class DelegateOperationConfiguration : IOperationConfiguration
        {
            private readonly Action<OperationDescription> configureOperation;

            public DelegateOperationConfiguration(Action<OperationDescription> configureOperation)
            {
                this.configureOperation = configureOperation;
            }

            public void Configure(OperationDescription operation)
            {
                this.configureOperation(operation);
            }
        }

        class DelegateInstanceFactory : IInstanceFactory
        {
            private readonly Func<Type, InstanceContext, Message, object> getInstance;

            private readonly Action<InstanceContext, object> releaseInstance;

            public DelegateInstanceFactory(Func<Type, InstanceContext, Message, object> getInstance, Action<InstanceContext, object> releaseInstance)
            {
                this.getInstance = getInstance;
                this.releaseInstance = releaseInstance;
            }

            public object GetInstance(Type serviceType, InstanceContext instanceContext, Message message)
            {
                return this.getInstance(serviceType, instanceContext, message);
            }

            public void ReleaseInstance(InstanceContext instanceContext, object service)
            {
                this.releaseInstance(instanceContext, service);
            }
        }

        class DelegateProcessorProvider : IProcessorProvider
        {
            private readonly Action<HttpOperationDescription, IList<Processor>, MediaTypeProcessorMode> requestProcessors;

            private readonly Action<HttpOperationDescription, IList<Processor>, MediaTypeProcessorMode> responseProcessors;

            public DelegateProcessorProvider(Action<HttpOperationDescription, IList<Processor>, MediaTypeProcessorMode> requestProcessors, Action<HttpOperationDescription, IList<Processor>, MediaTypeProcessorMode> responseProcessors)
            {
                this.requestProcessors = requestProcessors;
                this.responseProcessors = responseProcessors;
            }

            public void RegisterRequestProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
            {
                this.requestProcessors(operation, processors, mode);
            }

            public void RegisterResponseProcessorsForOperation(HttpOperationDescription operation, IList<Processor> processors, MediaTypeProcessorMode mode)
            {
                this.responseProcessors(operation, processors, mode);
            }
        }

        class DelegateServiceDescriptionFactory : IServiceDescriptionFactory
        {
            private readonly Func<Type, IDictionary<string, ContractDescription>, ServiceHost, ServiceDescription> factory;

            public DelegateServiceDescriptionFactory(Func<Type, IDictionary<string, ContractDescription>, ServiceHost, ServiceDescription> factory)
            {
                this.factory = factory;
            }

            public ServiceDescription CreateDescription(Type serviceType, IDictionary<string, ContractDescription> implementedContracts, ServiceHost host)
            {
                return factory(serviceType, implementedContracts, host);
            }
        }
    }
}