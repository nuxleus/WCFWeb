// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Dispatcher;

    public class MockPipelineContext : PipelineContext
    {
        public Action<Processor, object[]> OnReadAllInputsCalled { get; set; }
        public Func<ProcessorArgument, ProcessorArgument, object, object> OnWriteInputCalled { get; set; }

        public MockPipelineContext(Pipeline pipeline)
            : base(pipeline)
        {
        }
        protected override void OnWriteInput(ProcessorArgument outArgument, ProcessorArgument inArgument, object value)
        {
            if (this.OnWriteInputCalled != null)
            {
                value = this.OnWriteInputCalled(outArgument, inArgument, value);
            }
            base.OnWriteInput(outArgument, inArgument, value);
        }

        protected override object[] OnReadAllInputs(Processor processor)
        {
            object[] result = base.OnReadAllInputs(processor);
            if (this.OnReadAllInputsCalled != null)
            {
                this.OnReadAllInputsCalled(processor, result);
            }

            return result;
        }
    }

    public class MockPipelineContextWithOwnStore : PipelineContext
    {
        public MockPipelineContextWithOwnStore(Pipeline pipeline)
            : base(pipeline)
        {
            this.Cache = new Dictionary<ProcessorArgument, object>();
        }

        public object GetCachedValue(ProcessorArgument inArgument)
        {
            object value = null;
            this.Cache.TryGetValue(inArgument, out value);
            return value;
        }

        public Dictionary<ProcessorArgument, object> Cache { get; set; }

        protected override object[] OnReadAllInputs(Processor processor)
        {
            int nInputs = processor.InArguments.Count;
            object[] values = new object[nInputs];
            for (int i = 0; i < nInputs; ++i)
            {
                values[i] = this.GetCachedValue(processor.InArguments[i]);
            }
            return values;
        }

        protected override void OnWriteInput(ProcessorArgument outArgument, ProcessorArgument inArgument, object value)
        {
            this.Cache[inArgument] = value;
        }
    }
}
