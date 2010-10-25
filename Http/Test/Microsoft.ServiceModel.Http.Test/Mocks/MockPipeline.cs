// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Dispatcher;

    public class MockPipeline : Pipeline
    {
        public Action<ProcessorArgument, ProcessorArgument> OnBindArgumentsCalled { get; set; }
        public Action<ProcessorResult> OnErrorCalled { get; set; }
        public Func<PipelineContext> OnCreateContextCalled { get; set; }
        private MockPipelineContext context;

        public MockPipeline(IEnumerable<Processor> processors, IEnumerable<ProcessorArgument> inArguments, IEnumerable<ProcessorArgument> outArguments)
            : base(processors, inArguments, outArguments)
        {
        }

        public MockPipelineContext Context
        {
            get
            {
                if (this.context == null)
                {
                    this.context = new MockPipelineContext(this);
                }
                return this.context;
            }
        }

        protected override void OnBindArguments(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            if (this.OnBindArgumentsCalled != null)
            {
                this.OnBindArgumentsCalled(outArgument, inArgument);
            }

            base.OnBindArguments(outArgument, inArgument);
        }

        protected override PipelineContext OnCreateContext()
        {
            if (this.OnCreateContextCalled != null)
            {
                PipelineContext context = this.OnCreateContextCalled();

                // Mock semantics: if callee returns a context, use it.
                // If they return null, call the base
                if (context != null)
                {
                    return context;
                }

                return base.OnCreateContext();
            }

            // Default is to create a mock context
            return this.Context;
        }

        protected override void OnError(ProcessorResult result)
        {
            if (this.OnErrorCalled != null)
            {
                this.OnErrorCalled(result);
            }
        }
    }
}
