// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Dispatcher;

    public class MockPipelineBuilder : MockPipelineBuilder<Pipeline>
    {
    }

    public class MockPipelineBuilderOfMockPipeline : PipelineBuilder<MockPipeline>
    {
        protected override MockPipeline OnCreatePipeline(IEnumerable<Processor> processors, IEnumerable<ProcessorArgument> inArguments, IEnumerable<ProcessorArgument> outArguments)
        {
            return new MockPipeline(processors, inArguments, outArguments);
        }
    }

    public class MockPipelineBuilder<TPipeline> : PipelineBuilder<TPipeline> where TPipeline : Pipeline
    {
        public Func<IEnumerable<Processor>, IEnumerable<Processor>> OnFilterCalled { get; set; }
        public Func<IEnumerable<Processor>, IEnumerable<Processor>> OnFilterProcessors { get; set; }
        public Func<TPipeline,TPipeline> OnInitializeCalled { get; set; }
        public Func<IEnumerable<Processor>, IEnumerable<ProcessorArgument>, IEnumerable<ProcessorArgument>, TPipeline> OnCreatePipelineCalled { get; set; }
        public Func<IEnumerable<Processor>, IEnumerable<Processor>> OnOrderCalled { get; set; }
        public Func<ProcessorArgument, ProcessorArgument, bool?> OnShouldArgumentBindCalled { get; set; }
        public Func<ProcessorArgument, ProcessorArgument> OnShouldArgumentBindInArg { get; set; }
        public Func<ProcessorArgument, ProcessorArgument> OnShouldArgumentBindOutArg { get; set; }
        public Func<TPipeline, bool> OnValidateCalled { get; set; }
        public Func<Processor, Processor, ProcessorExecutionOrder, ProcessorExecutionOrder> OnGetRelativeExecutionOrderCalled { get; set; }
        public Func<Processor, Processor> OnGetRelativeExecutionOrderFirstProcessor { get; set; }
        public Func<Processor, Processor> OnGetRelativeExecutionOrderSecondProcessor { get; set; }
        public Func<IEnumerable<Processor>, IEnumerable<Processor>> OnCreatePipelineProcessors {get;set;}
        public Func<IEnumerable<ProcessorArgument>, IEnumerable<ProcessorArgument>> OnCreatePipelineInArgs { get; set; }
        public Func<IEnumerable<ProcessorArgument>, IEnumerable<ProcessorArgument>> OnCreatePipelineOutArgs { get; set; }

        protected override IEnumerable<Processor> OnFilter(IEnumerable<Processor> processors)
        {
            if (this.OnFilterProcessors != null)
            {
                processors = this.OnFilterProcessors(processors);
            }

            if (this.OnFilterCalled != null)
            {
                return this.OnFilterCalled(processors);
            }

            return base.OnFilter(processors);
        }

        protected override void OnInitialize(TPipeline pipeline)
        {
            if (this.OnInitializeCalled != null)
            {
                pipeline = this.OnInitializeCalled(pipeline);
            }

            base.OnInitialize(pipeline);
        }

        protected override TPipeline OnCreatePipeline(IEnumerable<Processor> processors, IEnumerable<ProcessorArgument> inArguments, IEnumerable<ProcessorArgument> outArguments)
        {
            if (this.OnCreatePipelineProcessors != null)
            {
                processors = this.OnCreatePipelineProcessors(processors);
            }

            if (this.OnCreatePipelineInArgs != null)
            {
                inArguments = this.OnCreatePipelineInArgs(inArguments);
            }

            if (this.OnCreatePipelineOutArgs != null)
            {
                outArguments = this.OnCreatePipelineOutArgs(outArguments);
            }

            if (this.OnCreatePipelineCalled != null)
            {
                return this.OnCreatePipelineCalled(processors, inArguments, outArguments);
            }

            return base.OnCreatePipeline(processors, inArguments, outArguments);
        }

        protected override ProcessorExecutionOrder OnGetRelativeExecutionOrder(Processor firstProcessor, Processor secondProcessor)
        {
            if (this.OnGetRelativeExecutionOrderFirstProcessor!= null)
            {
                firstProcessor = this.OnGetRelativeExecutionOrderFirstProcessor(firstProcessor);
            }

            if (this.OnGetRelativeExecutionOrderSecondProcessor != null)
            {
                secondProcessor = this.OnGetRelativeExecutionOrderSecondProcessor(secondProcessor);
            }

            ProcessorExecutionOrder order = base.OnGetRelativeExecutionOrder(firstProcessor, secondProcessor);

            if (this.OnGetRelativeExecutionOrderCalled != null)
            {
                order = this.OnGetRelativeExecutionOrderCalled(firstProcessor, secondProcessor, order);
            }

            return order;
        }

        protected override IEnumerable<Processor> OnOrder(IEnumerable<Processor> processors)
        {
            if (this.OnOrderCalled != null)
            {
                return this.OnOrderCalled(processors);
            }

            return base.OnOrder(processors);
        }

        protected override bool OnShouldArgumentsBind(ProcessorArgument outArgument, ProcessorArgument inArgument)
        {
            if (this.OnShouldArgumentBindInArg != null)
            {
                inArgument = this.OnShouldArgumentBindInArg(inArgument);
            }

            if (this.OnShouldArgumentBindOutArg != null)
            {
                outArgument = this.OnShouldArgumentBindOutArg(outArgument);
            }

            if (this.OnShouldArgumentBindCalled != null)
            {
                // Null response means test wants us to call base
                bool? result = this.OnShouldArgumentBindCalled(outArgument, inArgument);
                if (result.HasValue)
                {
                    return result.Value;
                }
            }

            return base.OnShouldArgumentsBind(outArgument, inArgument);
        }

        protected override void OnValidate(TPipeline pipeline)
        {
            if (this.OnValidateCalled != null)
            {
                // True return means test wants us to call base
                if (!this.OnValidateCalled(pipeline))
                {
                    return;
                }
            }

            base.OnValidate(pipeline);
        }
    }
}
