// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Http;
    using Microsoft.Http.Headers;

    public class HttpPipelineFormatter : HttpMessageFormatter
    {
        public const string ArgumentHttpRequestMessage = "HttpRequestMessage";
        public const string ArgumentHttpResponseMessage = "HttpResponseMessage";
        public const string ArgumentUri = "Uri";
        public const string ArgumentRequestHeaders = "RequestHeaders";
        public const string ArgumentResponseHeaders = "ResponseHeaders";

        private Processor[] requestProcessors;
        private Processor[] responseProcessors;

        private Pipeline requestPipeline;
        private Pipeline responsePipeline;
        private MessageProperties messageProperties;

        [SuppressMessage("Microsoft.Design", "CA1026", Justification = "Parameters cannot be defaulted")]
        public HttpPipelineFormatter(Processor[] requestProcessors, Processor[] responseProcessors, HttpOperationDescription operationDescription, MessageProperties messageProperties = null)
        {
            this.messageProperties = messageProperties;
            if (requestProcessors == null)
            {
                throw new ArgumentNullException("requestProcessors");
            }

            this.requestProcessors = requestProcessors;

            if (responseProcessors == null)
            {
                throw new ArgumentNullException("responseProcessors");
            }

            if (operationDescription == null)
            {
                throw new ArgumentNullException("operationDescription");
            }

            this.responseProcessors = responseProcessors;
            this.requestProcessors = requestProcessors;
            this.OperationDescription = operationDescription;
        }

        public HttpOperationDescription OperationDescription { get; set; }

        public PipelineBuilder Builder { get; set; }

        private MessageProperties MessageProperties
        {
            get
            {
                if (this.messageProperties != null)
                {
                    return this.messageProperties;
                }

                return OperationContext.Current.IncomingMessageProperties;
            }
        }

        public void Initialize()
        {
            var builder = this.Builder;
            if (builder == null)
            {
                builder = new PipelineBuilder();
            }

            this.requestPipeline = builder.Build(this.requestProcessors, GetRequestInArguments(), this.GetRequestOutArguments());
            this.responsePipeline = builder.Build(this.responseProcessors, this.GetResponseInArguments(), Enumerable.Empty<ProcessorArgument>());
        }
        
        protected override void SerializeReply(object[] parameters, object result, HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var httpMessageProperty = MessageProperties[HttpMessageProperty.Name] as HttpMessageProperty;
            this.responsePipeline.Execute(this.GetResponseInArgumentValues(httpMessageProperty.Request, response, result).ToArray());
            if (response.StatusCode == 0)
            {
                response.StatusCode = HttpStatusCode.OK;
            }
        }

        protected override HttpResponseMessage GetDefaultResponse()
        {
            var messageProperty = (HttpMessageProperty)MessageProperties[HttpMessageProperty.Name];
            var response = messageProperty.Response;

            return response;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Response message cannot be disposed")]
        protected override void DeserializeRequest(HttpRequestMessage message, object[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var httpMessageProperty = new HttpMessageProperty() { Request = message, Response = new HttpResponseMessage() };
            MessageProperties.Add(HttpMessageProperty.Name, httpMessageProperty);

            var result = this.requestPipeline.Execute(GetRequestInArgumentValues(message, httpMessageProperty.Response).ToArray());
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = result.Output[i];
            }
        }

        private static List<ProcessorArgument> GetRequestInArguments()
        {
            var args = new List<ProcessorArgument>();
            args.Add(new ProcessorArgument(ArgumentHttpRequestMessage, typeof(HttpRequestMessage)));
            args.Add(new ProcessorArgument(ArgumentHttpResponseMessage, typeof(HttpResponseMessage)));
            args.Add(new ProcessorArgument(ArgumentUri, typeof(Uri)));
            args.Add(new ProcessorArgument(ArgumentRequestHeaders, typeof(RequestHeaders)));
            args.Add(new ProcessorArgument(ArgumentResponseHeaders, typeof(ResponseHeaders)));
            return args;
        }

        private static List<object> GetRequestInArgumentValues(HttpRequestMessage request, HttpResponseMessage response)
        {
            var args = new List<object>();
            args.Add(request);
            args.Add(response);
            args.Add(request.Uri);
            args.Add(request.Headers);
            args.Add(response.Headers);
            return args;
        }

        private List<ProcessorArgument> GetRequestOutArguments()
        {
            var args = new List<ProcessorArgument>();
            foreach (var parameter in this.OperationDescription.InputParameters)
            {
                string parameterName = null;

                if (parameter.ParameterType == typeof(HttpRequestMessage))
                {
                    parameterName = ArgumentHttpRequestMessage;
                }
                else if (parameter.ParameterType == typeof(HttpResponseMessage))
                {
                    parameterName = ArgumentHttpResponseMessage;
                }
                else
                {
                    parameterName = parameter.Name;
                }

                args.Add(new ProcessorArgument(parameterName, parameter.ParameterType));
            }

            return args;
        }

        private List<ProcessorArgument> GetResponseInArguments()
        {
            var returnValue = this.OperationDescription.ReturnValue;
            var args = GetRequestInArguments();
            
            if (returnValue != null) 
            {
                args.Add(new ProcessorArgument(returnValue.Name, returnValue.ParameterType));
            }

            return args;
        }

        private List<object> GetResponseInArgumentValues(HttpRequestMessage request, HttpResponseMessage response, object returnValue)
        {
            var returnValueParameter = this.OperationDescription.ReturnValue;
            var values = GetRequestInArgumentValues(request, response);
            if (returnValueParameter != null)
            {
                values.Add(returnValue);
            }

            return values;
        }
    }
}
