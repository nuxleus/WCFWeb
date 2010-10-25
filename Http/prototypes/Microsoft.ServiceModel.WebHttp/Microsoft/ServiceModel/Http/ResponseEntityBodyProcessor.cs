// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Dispatcher;
    using Dispatcher;
    using Microsoft.Http;

    public class ResponseEntityBodyProcessor : Processor
    {
        private List<string> contentTypes;

        protected override void OnInitialize()
        {
            this.contentTypes = new List<string>();
            var formatters = this.ContainingCollection.OfType<MediaTypeProcessor>();
            foreach (var formatter in formatters)
            {
                foreach (var mediaType in formatter.SupportedMediaTypes)
                {
                    if (mediaType != null)
                    {
                        this.contentTypes.Add(mediaType);
                    }
                }
            }
        }
 
        protected override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            var arguments = new List<ProcessorArgument>();
            arguments.Add(new ProcessorArgument(HttpPipelineFormatter.ArgumentHttpRequestMessage, typeof(HttpRequestMessage)));
            arguments.Add(new ProcessorArgument(HttpPipelineFormatter.ArgumentHttpResponseMessage, typeof(HttpResponseMessage)));
            return arguments;
        }

        protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            var argument = new ProcessorArgument(MediaTypeProcessor.ArgumentContentType, typeof(string));
            return new List<ProcessorArgument> { argument };
        }

        protected override ProcessorResult OnExecute(object[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var request = (HttpRequestMessage)input[0];
            var response = (HttpResponseMessage)input[1];
            string accept = request.Headers.Accept.ToString();
            string contentType = response.Headers.ContentType;
            if (contentType == null || contentType == String.Empty)
            {
                contentType = ContentNegotiationHelper.GetBestMatch(accept, this.contentTypes).MediaType;
            }

            var output = new object[] { contentType };
            return new ProcessorResult { Output = output };
        }
    }
}
