// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Dispatcher;
    using Http;

    public class HttpEndpointBehavior : IEndpointBehavior
    {
        private static readonly HashSet<string> defaultOperationBehaviorNames = new HashSet<string>()
            {
                "System.ServiceModel.Dispatcher.OperationInvokerBehavior",
                "System.ServiceModel.OperationBehaviorAttribute"
            };

        private Uri baseAddress;
        private IProcessorProvider processorProvider;
        private HttpHostConfiguration configuration;

        public HttpEndpointBehavior(HttpHostConfiguration configuration)
        {
            this.configuration = configuration;
            this.processorProvider = configuration.ProcessorProvider;
        }

        public Processor[] GetRequestProcessors(HttpOperationDescription operation)
        {
            var processors = new List<Processor>();
            processors.Add(new UriTemplateProcessor(this.baseAddress, new UriTemplate(operation.GetUriTemplateString())));
            processors.Add(new XmlProcessor(operation, MediaTypeProcessorMode.Request));
            if (this.processorProvider != null)
            {
                this.processorProvider.RegisterRequestProcessorsForOperation(operation, processors, MediaTypeProcessorMode.Request);
            }

            return processors.ToArray();
        }

        public Processor[] GetResponseProcessors(HttpOperationDescription operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var processors = new List<Processor>();
            processors.Add(new ResponseEntityBodyProcessor());
            processors.Add(new XmlProcessor(operation, MediaTypeProcessorMode.Response));
            if (this.processorProvider != null)
            {
                this.processorProvider.RegisterResponseProcessorsForOperation(operation, processors, MediaTypeProcessorMode.Response);
            }

            return processors.ToArray();
        }

        public virtual TBehavior Cast<TBehavior>() where TBehavior : IEndpointBehavior
        {
            if (typeof(TBehavior).IsAssignableFrom(this.GetType()))
            {
                return (TBehavior)(IEndpointBehavior)this;
            }

            return default(TBehavior);
        }

        void IEndpointBehavior.AddBindingParameters(
                                   ServiceEndpoint endpoint, 
                                   BindingParameterCollection bindingParameters)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033",
            Justification = "Client-side behaviors are not supported.")]
        void IEndpointBehavior.ApplyClientBehavior(
                                   ServiceEndpoint endpoint, 
                                   ClientRuntime clientRuntime)
        {
            // TODO:  Move to Resources
            throw new NotSupportedException(string.Format(
                CultureInfo.CurrentCulture,
                "The HttpEndpointBehavior of type '{0}' is not supported on the client.",
                this.GetType().FullName));
        }

        void IEndpointBehavior.ApplyDispatchBehavior(
                                   ServiceEndpoint endpoint, 
                                   EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            this.baseAddress = endpoint.Address.Uri;
            InitializeEndpointDispatcher(endpointDispatcher);
            DispatchRuntime dispatchRuntime = endpointDispatcher.DispatchRuntime;
            var contract = endpoint.Contract;
            IEnumerable<IOperationBehavior> defaultOperationBehaviors =
                GetDefaultOperationBehaviors(contract);

            if (this.configuration.InstanceFactory != null)
            {
                dispatchRuntime.InstanceProvider = new InstanceFactoryProvider(contract.ContractType, this.configuration.InstanceFactory);
            }

            var instanceProvider = this.OnGetInstanceProvider(endpoint, dispatchRuntime);
            if (instanceProvider != null)
            {
                dispatchRuntime.InstanceProvider = instanceProvider;
            }

            dispatchRuntime.OperationSelector = new MethodAndUriTemplateOperationSelector(endpoint);
            
            Dictionary<string, DispatchOperation> dispatchOperations =
                dispatchRuntime.Operations.ToDictionary(d => d.Name);

            dispatchRuntime.Operations.Clear();
            
            foreach (OperationDescription operationDescription in contract.Operations)
            {
                string name = operationDescription.Name;
                DispatchOperation dispatchOperation = null;
                if (!dispatchOperations.TryGetValue(name, out dispatchOperation))
                {
                    dispatchOperation = operationDescription.IsOneWay ?
                        new DispatchOperation(dispatchRuntime, name, string.Empty) :
                        new DispatchOperation(dispatchRuntime, name, string.Empty, string.Empty);

                    foreach (IOperationBehavior defaultBehavior in defaultOperationBehaviors)
                    {
                        defaultBehavior.ApplyDispatchBehavior(
                                            operationDescription,
                                            dispatchOperation);
                    }
                }

                var httpOperationDescription = new HttpOperationDescription(operationDescription);

                dispatchOperation.Formatter = this.OnGetFormatter(httpOperationDescription, dispatchOperation);

                var invoker = this.OnGetInvoker(httpOperationDescription, dispatchOperation);
                if (invoker != null)
                {
                    dispatchOperation.Invoker = invoker;
                }

                dispatchRuntime.Operations.Add(dispatchOperation);
            }
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }
        }

        protected virtual HttpMessageFormatter OnGetFormatter(HttpOperationDescription description, DispatchOperation operation)
        {
            var requestProcessors = this.GetRequestProcessors(description);
            var responseProcessors = this.GetResponseProcessors(description);
            var pipelineFormatter = new HttpPipelineFormatter(requestProcessors, responseProcessors, description);
            pipelineFormatter.Initialize();
            return pipelineFormatter;
        }

        protected virtual IOperationInvoker OnGetInvoker(HttpOperationDescription description, DispatchOperation operation)
        {
            return null;
        }

        protected virtual IInstanceProvider OnGetInstanceProvider(ServiceEndpoint endpoint, DispatchRuntime runtime)
        {
            return null;            
        }

        private static void InitializeEndpointDispatcher(EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.AddressFilter =
                new PrefixEndpointAddressMessageFilter(endpointDispatcher.EndpointAddress);
            endpointDispatcher.ContractFilter = new MatchAllMessageFilter();
        }

        private static IEnumerable<IOperationBehavior> GetDefaultOperationBehaviors(
                                                   ContractDescription contract)
        {
            Debug.Assert(contract != null, "Parameter must not be null.");

            return contract.Operations[0].Behaviors.Where(b => IsDefaultOperationBehavior(b));
        }

        private static bool IsDefaultOperationBehavior(IOperationBehavior behavior)
        {
            Debug.Assert(behavior != null, "Parameter must not be null.");
            string name = behavior.GetType().FullName;
            return defaultOperationBehaviorNames.Contains(name);
        }
    }
}
