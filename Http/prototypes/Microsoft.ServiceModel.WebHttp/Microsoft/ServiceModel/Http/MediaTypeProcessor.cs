// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Dispatcher;
    using Microsoft.Http;

    public enum MediaTypeProcessorMode
    {
        Request,
        Response
    }

    public abstract class MediaTypeProcessor : Processor
    {
        public const string ArgumentContentType = "ContentType";
        private MediaTypeProcessorMode mode;
        private bool ignore;

        protected MediaTypeProcessor(HttpOperationDescription operation, MediaTypeProcessorMode mode)
        {
            if (mode == MediaTypeProcessorMode.Request)
            {
                this.Parameter = operation.GetBodyParameter();
            }
            else
            {
                this.Parameter = operation.ReturnValue;
            }

            if (this.Parameter != null && (this.Parameter.ParameterType == typeof(HttpRequestMessage) ||
                      this.Parameter.ParameterType == typeof(HttpResponseMessage)))
            {
                this.ignore = true;
                this.Parameter = null;
            }

            this.mode = mode;
        }

        public abstract IEnumerable<string> SupportedMediaTypes { get; }

        protected HttpParameterDescription Parameter { get; set; }

        public abstract void WriteToStream(object instance, Stream stream, HttpRequestMessage request);

        public abstract object ReadFromStream(Stream stream, HttpRequestMessage request);

        protected override ProcessorResult OnExecute(object[] input)
        {
            ProcessorResult result = null;

            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (this.ignore)
            {
                result = new ProcessorResult();
            }

            if (this.mode == MediaTypeProcessorMode.Request)
            {
                result = this.ExecuteRequest(input);
            }
            else
            {
                this.ExecuteResponse(input);
            }

            if (result == null)
            {
                result = new ProcessorResult();
            }

            return result;
        }

        protected override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            var arguments = new List<ProcessorArgument>();
            if (this.mode == MediaTypeProcessorMode.Request)
            {
                arguments.Add(new ProcessorArgument(HttpPipelineFormatter.ArgumentHttpRequestMessage, typeof(HttpRequestMessage)));
            }
            else
            {
                if (this.Parameter != null)
                {
                    arguments.Add(new ProcessorArgument(this.Parameter.Name, this.Parameter.ParameterType));
                }

                arguments.Add(new ProcessorArgument(ArgumentContentType, typeof(string)));
                arguments.Add(new ProcessorArgument(HttpPipelineFormatter.ArgumentHttpResponseMessage, typeof(HttpResponseMessage)));
                arguments.Add(new ProcessorArgument(HttpPipelineFormatter.ArgumentHttpRequestMessage, typeof(HttpRequestMessage)));
            }

            return arguments.ToArray();
        }

        protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            var arguments = new List<ProcessorArgument>();
            if (this.mode == MediaTypeProcessorMode.Request && this.Parameter != null)
            {
                arguments.Add(new ProcessorArgument(this.Parameter.Name, this.Parameter.ParameterType));
            }

            return arguments.ToArray();
        }

        private ProcessorResult ExecuteRequest(object[] input)
        {
            var request = (HttpRequestMessage)input[0];
            ProcessorResult result = null;

            if (request.Headers.ContentType != null && this.SupportedMediaTypes.Contains(request.Headers.ContentType))
            {
                result = new ProcessorResult<object> { Output = this.ReadFromStream(request.Content.ReadAsStream(), request) };
            }

            return result;
        }

        private void ExecuteResponse(object[] input)
        {
            int i = this.Parameter != null ? 0 : 1;
            object instance = null;

            if (i == 0)
            {
                instance = input[0];
            }

            var contentType = (string)input[++i];
            var response = (HttpResponseMessage)input[++i];
            var request = (HttpRequestMessage)input[++i];

            if (instance != null)
            {
                if (this.SupportedMediaTypes.Contains(contentType))
                {
                    response.Content = HttpContent.Create(s => this.WriteToStream(instance, s, request));
                    response.Headers.ContentType = contentType;
                }
            }
        }
    }
}
