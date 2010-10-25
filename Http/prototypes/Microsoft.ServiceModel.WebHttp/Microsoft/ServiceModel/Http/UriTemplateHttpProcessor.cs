// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel.Dispatcher;
    using Dispatcher;

    public class UriTemplateProcessor : Processor
    {
        private static Type stringType = typeof(string);

        private UriTemplate template;
        private Uri baseAddress;

        public UriTemplateProcessor(Uri baseAddress, UriTemplate template)
        {
            this.baseAddress = baseAddress;
            this.template = template;
        }

        protected override IEnumerable<ProcessorArgument> OnGetInArguments()
        {
            var args = new List<ProcessorArgument>();
            args.Add(new ProcessorArgument(HttpPipelineFormatter.ArgumentUri, typeof(Uri)));
            return args;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "necessary for parameter match")]
        protected override IEnumerable<ProcessorArgument> OnGetOutArguments()
        {
            var args = this.template.PathSegmentVariableNames
                    .Select(s => new ProcessorArgument(
                                          s.ToLowerInvariant(),
                                          stringType));
            return args;
        }

        protected override ProcessorResult OnExecute(object[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            UriTemplateMatch match = this.template.Match(this.baseAddress, (Uri)input[0]);
            Debug.Assert(match != null, "The operation selector must have gotten a match.");
            object[] output = new object[match.BoundVariables.Count];
            if (this.OutArguments.Count > 0)
            {
                for (int i = 0; i < match.BoundVariables.Count; i++)
                {
                    output[i] = match.BoundVariables.GetValues(i).First();
                }
            }
            else
            {
                output = new object[0];
            }

            return new ProcessorResult { Output = output };
        }
    }
}
