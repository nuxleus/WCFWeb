// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PipelineContextTests
    {
        [TestMethod]
        [Description("PipelineContext ctor throws for null pipeline")]
        public void PipelineContext_Ctor_Null_Pipeline_Throws()
        {
            PipelineContext context;
            ExceptionAssert.ThrowsArgumentNull(
                "A null pipeline argument to the PipelineContext ctor should throw",
                "pipeline",
                () => context = new PipelineContext(null)
                );
        }

        [TestMethod]
        [Description("PipelineContext public ctor reuses PipelineContextInfo from pipeline")]
        public void PipelineContext_Ctor_Internal_Reuses_Pipeline_ContextInfo()
        {
            Processor processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(new[] { processor }, Enumerable.Empty<ProcessorArgument>(), Enumerable.Empty<ProcessorArgument>());
            PipelineContext context1 = new PipelineContext(pipeline);
            Assert.IsNotNull(context1.ContextInfo, "ContextInfo should have been created");

            PipelineContext context2 = new PipelineContext(pipeline);
            Assert.AreSame(context1.ContextInfo, context2.ContextInfo, "Expected 2nd context to reuse same ContextInfo");
        }

        [TestMethod]
        [Description("PipelineContext internal ctor creates PipelineContextInfo if null is specified.")]
        public void PipelineContext_Ctor_Internal_Null_ContextInfo_Creates()
        {
            Processor processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(new[] { processor }, Enumerable.Empty<ProcessorArgument>(), Enumerable.Empty<ProcessorArgument>());
            PipelineContext context = new PipelineContext(pipeline, null);
            Assert.IsNotNull(context.ContextInfo, "Null contextInfo to ctor should have created one");
        }

        [TestMethod]
        [Description("PipelineContext internal ctor accepts new ContextInfo for testing.")]
        public void PipelineContext_Ctor_Internal_Accepts_ContextInfo()
        {
            Processor processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(new[] { processor }, Enumerable.Empty<ProcessorArgument>(), Enumerable.Empty<ProcessorArgument>());
            PipelineContextInfo info = new PipelineContextInfo(pipeline);
            PipelineContext context = new PipelineContext(pipeline, info);
            Assert.AreSame(info, context.ContextInfo, "Ctor should have used the ContextInfo we passed int.");
        }

        [TestMethod]
        [Description("PipelineContext.ReadAllInputs throws if null processor")]
        public void PipelineContext_ReadAllInputs_Throws_If_Null_Processor()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.ThrowsArgumentNull(
                "PipelineContext should throw if null processor specified",
                "processor",
                () => context.ReadAllInputs(null)
                );
        }

        [TestMethod]
        [Description("PipelineContext.ReadAllInputs throws if processor not in pipeline")]
        public void PipelineContext_ReadAllInputs_Throws_If_Processor_Not_In_Pipeline()
        {
            Processor processor = new MockProcessor1();
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "PipelineContext should throw if processor from different pipeline specified",
                () => context.ReadAllInputs(processor),
                "processor"
                );
        }

        [TestMethod]
        [Description("PipelineContext.ReadInput throws if null processor argument")]
        public void PipelineContext_ReadInput_Throws_If_Null_ProcessorArgument()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.ThrowsArgumentNull(
                "PipelineContext should throw if null processor argument specified",
                "inArgument",
                () => context.ReadInput(null)
                );
        }

        [TestMethod]
        [Description("PipelineContext.ReadInput throws if pass in containerless processor argument")]
        public void PipelineContext_ReadInput_Throws_If_Containerless_ProcessorArgument()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "PipelineContext should throw if processor argument belongs to no collection",
                () => context.ReadInput(new ProcessorArgument("foo", typeof(string))),
                "inArgument"
                );
        }

        [TestMethod]
        [Description("PipelineContext.ReadInput throws if use different pipeline's processor argument")]
        public void PipelineContext_ReadInput_Throws_If_External_ProcessorArgument()
        {
            Processor processor = new MockProcessor1();
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "PipelineContext should throw if processor argument belongs to different pipeline",
                () => context.ReadInput(processor.InArguments[0]),
                "inArgument"
                );
        }

        [TestMethod]
        [Description("PipelineContext.ReadInput returns WriteInput value")]
        public void PipelineContext_ReadInput_Returns_WriteInput()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            MockPipelineContext context = pipeline.Context;
            context.WriteInput(pipeline.Processors[1].InArguments[0], 7);
            object value = context.ReadInput(pipeline.Processors[1].InArguments[0]);
            Assert.AreEqual(7, value, "Expected processor 1 to have this input value after we wrote it");
        }

        [TestMethod]
        [Description("PipelineContext.ReadInput returns expected result after execution")]
        public void PipelineContext_ReadInput_Returns_Result_After_Execution()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            MockPipelineContext context = pipeline.Context;
            object value = context.ReadInput(pipeline.Processors[1].InArguments[0]);
            Assert.IsNull(value, "Expected null prior to execution");
            ProcessorResult result = pipeline.Execute(new object[] { 7 });
            value = context.ReadInput(pipeline.Processors[1].InArguments[0]);
            Assert.AreEqual(7, value, "Expected processor 1 to have this input value after execution");
        }

        [TestMethod]
        [Description("PipelineContext.WriteInput throws if null processor argument")]
        public void PipelineContext_WriteInput_Throws_If_Null_ProcessorArgument()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.ThrowsArgumentNull(
                "PipelineContext should throw if null processor argument specified",
                "inArgument",
                () => context.WriteInput(null, new object())
                );
        }

        [TestMethod]
        [Description("PipelineContext.WriteInput throws if pass in containerless processor argument")]
        public void PipelineContext_WriteInput_Throws_If_Containerless_ProcessorArgument()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "PipelineContext should throw if processor argument belongs to no collection",
                () => context.WriteInput(new ProcessorArgument("foo", typeof(string)), new object()),
                "inArgument"
                );
        }

        [TestMethod]
        [Description("PipelineContext.WriteInput throws if use different pipeline's processor argument")]
        public void PipelineContext_WriteInput_Throws_If_External_ProcessorArgument()
        {
            Processor processor = new MockProcessor1();
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);
            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "PipelineContext should throw if processor argument belongs to different pipeline",
                () => context.WriteInput(processor.InArguments[0], new object()),
                "inArgument"
                );
        }


        [TestMethod]
        [Description("PipelineContext.CurrentProcessor initializes and advances properly")]
        public void PipelineContext_CurrentProcessor_Property()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);

            Assert.IsNull(context.CurrentProcessor, "CurrentProcessor should be null before advance to first");

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to first");
            Assert.AreSame(pipeline.Processors[0], context.CurrentProcessor, "First processor incorrect");

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to second");
            Assert.AreSame(pipeline.Processors[1], context.CurrentProcessor, "Second processor incorrect");

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to third");
            Assert.AreSame(pipeline.Processors[2], context.CurrentProcessor, "Third processor incorrect");

            Assert.IsFalse(context.AdvanceToNextProcessor(), "Moving off end should have returned false");
            Assert.IsNull(context.CurrentProcessor, "CurrentProcessor should be null after final advance");
        }

        [TestMethod]
        [Description("PipelineContext.ReadAllInputs works correctly")]
        public void PipelineContext_ReadAllInputs()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = pipeline.Context;

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to first");

            object[] input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Inputs for first processor should not be null");
            Assert.AreEqual(0, input.Length, "First processor should have zero inputs");

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to second");
            input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Inputs for second processor should not be null");
            Assert.AreEqual(1, input.Length, "Second processor should have one input");

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to third");
            input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Inputs for third processor should not be null");
            Assert.AreEqual(1, input.Length, "Third processor should have one input");
        }

        [TestMethod]
        [Description("PipelineContext.OnReadAllInputs can be overridden and values modified")]
        public void PipelineContext_OnReadAllInputs_Overridable()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            MockPipelineContext context = pipeline.Context;
            bool processorCalled = false;

            context.OnReadAllInputsCalled = (inProc, inValues) =>
            {
                // Inject a '3' into the inputs for proc 1
                if (inProc == pipeline.Processors[1])
                {
                    processorCalled = true;
                    Assert.AreEqual(4, inValues[0], "Expected to see '4' as the pipeline input");
                    inValues[0] = 3;
                }
            };

            ProcessorResult procResult = pipeline.Execute(new object[] { 4 });
            Assert.IsNotNull(procResult.Output, "Expected output array");
            Assert.AreEqual(1, procResult.Output.Length, "Output size mismatch");
            Assert.AreEqual(3.ToString(), procResult.Output[0], "Pipeline did not use context we injected");

            Assert.IsTrue(processorCalled, "Our ReadAllInputs override was not called");
        }

        [TestMethod]
        [Description("PipelineContext.SetProcessorOutput works correctly")]
        public void PipelineContext_SetProcessorOutput()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to first");

            // Mimic entry processor producing its output of an intValue=5
            context.SetProcessorOutputs(context.CurrentProcessor, new object[] { 5 });

            // This should have been copied to the input slots for second processor
            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to second");
            object[] input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Input cannot be null");
            Assert.AreEqual(1, input.Length, "Expected 1 element in input");
            Assert.AreEqual(5, input[0], "Input should have contained a 5");

            // Mimic processor 1 producing an output of theResult="aString"
            context.SetProcessorOutputs(context.CurrentProcessor, new object[] { "aString" });

            // This should have been copied to the input slots for third processor
            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to third");
            input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Input cannot be null");
            Assert.AreEqual(1, input.Length, "Expected 1 element in input");
            Assert.AreEqual("aString", input[0], "Input should have contained this string");
        }

        [TestMethod]
        [Description("PipelineContext.SetProcessorOutput fans out correctly to 2 processors bound to same output")]
        public void PipelineContext_SetProcessorOutput_Fanout_2_Processors()
        {
            Pipeline pipeline = TestPipelines.CreateDoubleMockProcessor1Pipeline();
            PipelineContext context = new PipelineContext(pipeline);

            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to first");

            // Mimic entry processor producing its output of an intValue=5
            context.SetProcessorOutputs(context.CurrentProcessor, new object[] { 5 });

            // This should have been copied to the input slots for second processor
            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to second");
            object[] input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Input cannot be null");
            Assert.AreEqual(1, input.Length, "Expected 1 element in input");
            Assert.AreEqual(5, input[0], "Input should have contained a 5");

            // Mimic second processor setting its output to "aString1"
            context.SetProcessorOutputs(context.CurrentProcessor, new object[] { "aString1" });

            // Move to 3rd processor and ensure it got the same inputs as 2nd
            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to third");
            input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Input cannot be null");
            Assert.AreEqual(1, input.Length, "Expected 1 element in input");
            Assert.AreEqual(5, input[0], "Input should have contained a 5");

            // Mimic third processor setting its output to "aString2"
            context.SetProcessorOutputs(context.CurrentProcessor, new object[] { "aString2" });

            // This should have been copied to the input slots for final processor
            Assert.IsTrue(context.AdvanceToNextProcessor(), "Failed to advance to third");
            input = context.ReadAllInputs(context.CurrentProcessor);
            Assert.IsNotNull(input, "Input cannot be null");
            Assert.AreEqual(2, input.Length, "Expected 2 elements in input");
            Assert.AreEqual("aString1", input[0], "Input[0] should have contained this string");
            Assert.AreEqual("aString2", input[1], "Input[1] should have contained this string");
        }

        [TestMethod]
        [Description("PipelineContext can redirect to different storage")]
        public void PipelineContext_Can_Redirect_Storage()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();

            // Create our own special context and redirect to it when pipeline executes
            MockPipelineContextWithOwnStore context = new MockPipelineContextWithOwnStore(pipeline);
            pipeline.OnCreateContextCalled = () => context;

            ProcessorResult procResult = pipeline.Execute(new object[] { 4 });
            Assert.IsNotNull(procResult.Output, "Expected output array");
            Assert.AreEqual(1, procResult.Output.Length, "Output size mismatch");
            Assert.AreEqual(4.ToString(), procResult.Output[0], "Pipeline did produce expected value");

            Dictionary<ProcessorArgument, object> cache = context.Cache;
            Assert.AreEqual(2, cache.Count, "Expected 2 cached input values");


        }
    }
}
