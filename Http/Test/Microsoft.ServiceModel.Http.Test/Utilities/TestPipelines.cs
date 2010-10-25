// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Utilities
{
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;

    public class TestPipelines
    {
        /// <summary>
        /// Creates a Pipeline with no actual processor other than binding the pipeline inputs to the
        /// pipeline outputs. 
        /// </summary>
        /// <returns></returns>
        public static MockPipeline CreateEmptyPipeline()
        {
            MockPipeline pipeline = new MockPipeline(
                                    new Processor[0],
                                    new[] { new ProcessorArgument("inValue", typeof(string), new object[0]) },
                                    new[] { new ProcessorArgument("outValue", typeof(string), new object[0]) }
                                    );

            pipeline.BindArgumentToPipelineInput("inValue", pipeline.Processors[1].InArguments[0]);
            pipeline.Initialize();
            return pipeline;
        }

        /// <summary>
        /// Creates a Pipeline with a single MockProcessor1 in it, binding its inputs to the pipeline inputs
        /// and its outputs to the pipeline outputs.
        /// </summary>
        /// <returns></returns>
        public static MockPipeline CreateMockProcessor1Pipeline()
        {
            MockPipeline pipeline = CreateMockProcessor1PipelineUninitialized();
            pipeline.Initialize();
            return pipeline;
        }

        /// <summary>
        /// Creates an uninitialized Pipeline with a single MockProcessor1 in it, binding its inputs to the pipeline inputs
        /// and its outputs to the pipeline outputs.
        /// </summary>
        /// <returns></returns>
        public static MockPipeline CreateMockProcessor1PipelineUninitialized()
        {
            Processor processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                                    new[] { processor },
                                    new[] { new ProcessorArgument("intValue", typeof(int), new object[0]) },
                                    new[] { new ProcessorArgument("theResult", typeof(string), new object[0]) }
                                    );

            pipeline.BindArgumentToPipelineInput("intValue", processor.InArguments[0]);
            pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], "theResult");
            return pipeline;
        }

        /// <summary>
        /// This pipeline has 2 MockProcessor1's in it, both binding their inputs to the pipeline's 'intValue'.
        /// The pipeline has 2 outputs, one bound to each processor's.
        /// </summary>
        /// <returns></returns>
        public static MockPipeline CreateDoubleMockProcessor1Pipeline()
        {
            Processor processor1 = new MockProcessor1();
            Processor processor2 = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                                    new[] { processor1, processor2 },
                                    new[] { new ProcessorArgument("intValue", typeof(int), new object[0]) },
                                    new[] { new ProcessorArgument("theResult1", typeof(string), new object[0]),
                                            new ProcessorArgument("theResult2", typeof(string), new object[0])}
                                    );

            // Pipeline["intValue"] --> Processor1[0]
            pipeline.BindArgumentToPipelineInput("intValue", processor1.InArguments[0]);

            // Pipeline["intValue"] --> Processor2[0]
            pipeline.BindArgumentToPipelineInput("intValue", processor2.InArguments[0]);

            // Processor1[0] --> Pipeline["theResult1"]
            pipeline.BindArgumentToPipelineOutput(processor1.OutArguments[0], "theResult1");

            // Processor2[0] --> Pipeline["theResult2"]
            pipeline.BindArgumentToPipelineOutput(processor2.OutArguments[0], "theResult2");
            pipeline.Initialize();
            return pipeline;
        }
    }
}
