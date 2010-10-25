// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Linq;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PipelineExecutionTests
    {
        [TestMethod]
        [Description("Pipeline.Execute throws if given null inputs")]
        public void Pipeline_Execute_Throws_Null_Inputs()
        {
            // Create a pipeline with only an entry and exit processor (no actual external processors).
            // The pipeline input is bound to the pipeline output.
            // This tests the zero-processor case as well as pipeline in/out binding
            Pipeline pipeline = TestPipelines.CreateEmptyPipeline();
            ProcessorResult result;
            ExceptionAssert.ThrowsArgumentNull(
                "Pipeline.Execute should throw if the inputs are null",
                "input",
                () => result = pipeline.Execute(null)
                );
        }

        [TestMethod]
        [Description("Pipeline.Execute throws if not initialized")]
        public void Pipeline_Execute_Throws_Not_Initialized()
        {
            // Create a pipeline with only an entry and exit processor (no actual external processors).
            // The pipeline input is bound to the pipeline output.
            // This tests the zero-processor case as well as pipeline in/out binding
            Pipeline pipeline = TestPipelines.CreateMockProcessor1PipelineUninitialized();
            ProcessorResult result;
            ExceptionAssert.ThrowsInvalidOperation(
                "Pipeline.Execute should throw if not initialized",
                () => result = pipeline.Execute(new object[1])
                );
        }

        [TestMethod]
        [Description("Pipeline executes correctly when zero processors")]
        public void Pipeline_Executes_When_Empty()
        {
            // Create a pipeline with only an entry and exit processor (no actual external processors).
            // The pipeline input is bound to the pipeline output.
            // This tests the zero-processor case as well as pipeline in/out binding
            Pipeline pipeline = TestPipelines.CreateEmptyPipeline();
            ProcessorResult result = pipeline.Execute(new object[] { "aString" });

            object[] output = result.Output;
            Assert.IsNotNull(output, "Processing output was null");
            Assert.AreEqual(1, output.Length, "Should have received 1 output");
            Assert.AreEqual("aString", output[0], "Should have seen this output");
        }

        [TestMethod]
        [Description("Pipeline executes a single processor with inputs and outputs correctly")]
        public void Pipeline_Executes_One_Input_Output_Processor()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            ProcessorResult result = pipeline.Execute(new object[] { 5 });

            object[] output = result.Output;
            Assert.IsNotNull(output, "Processing output was null");
            Assert.AreEqual(1, output.Length, "Should have received 1 output");
            Assert.AreEqual(5.ToString(), output[0], "Should have seen this output");
        }

        [TestMethod]
        [Description("Pipeline executes 2 processors accepting common input and delivering double output")]
        public void Pipeline_Executes_2_Processors_Common_In_Double_Out()
        {
            Pipeline pipeline = TestPipelines.CreateDoubleMockProcessor1Pipeline();

            // Override the OnExecute of both so we see unique outputs
            ((MockProcessor1) pipeline.Processors[1]).OnExecuteCalled = 
                i => new ProcessorResult<string>() { Output = "result1 for " + i };

            ((MockProcessor1)pipeline.Processors[2]).OnExecuteCalled =
                i => new ProcessorResult<string>() { Output = "result2 for " + i };

            ProcessorResult result = pipeline.Execute(new object[] { 5 });

            // This test has 2 MockProcessor1's, both bound to the single pipeline input 'intValue'.
            // Each processor output is bound to one of the 2 pipeline outputs
            object[] output = result.Output;
            Assert.IsNotNull(output, "Processing output was null");
            Assert.AreEqual(2, output.Length, "Should have received 1 output");
            Assert.AreEqual("result1 for 5", output[0], "Should have seen this output[0]");
            Assert.AreEqual("result2 for 5", output[1], "Should have seen this output[1]");
        }

        [TestMethod]
        [Description("Pipeline base class creates a PipelineContext")]
        public void Pipeline_Execute_Base_Creates_Context()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            bool wasCalled = false;

            // Called back from mock.  Null says "call the base.OnCreateContext"
            pipeline.OnCreateContextCalled = () =>
            {
                wasCalled = true;
                return null;
            };

            ProcessorResult result = pipeline.Execute(new object[] { 5 });

            Assert.IsTrue(wasCalled, "OnCreateContext was not called");
            object[] output = result.Output;
            Assert.IsNotNull(output, "Processing output was null");
            Assert.AreEqual(1, output.Length, "Should have received 1 output");
            Assert.AreEqual(5.ToString(), output[0], "Should have seen this output");
        }

        [TestMethod]
        [Description("Pipeline executes a processor that returns an error in ProcessorResult")]
        public void Pipeline_Executes_Error_Processor()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            ProcessorResult errorResult = null;

            // Override the OnExecute to return a failure
            ((MockProcessor1)pipeline.Processors[1]).OnExecuteCalled =
                i => new ProcessorResult<string>() { Status = ProcessorStatus.Error };

            // Override OnError to capture the error result
            ((MockProcessor1)pipeline.Processors[1]).OnErrorCalledAction = r => errorResult = r;

            // The final pipeline result should reflect that error status
            ProcessorResult result = pipeline.Execute(new object[] { 5 });
            Assert.IsNotNull(result, "Failed to get a result");
            Assert.AreEqual(ProcessorStatus.Error, result.Status, "Expected error status");

            Assert.IsNotNull(errorResult, "OnError should have been called");
        }

        [TestMethod]
        [Description("Pipeline executing a processor that returns null output array ignores the output")]
        public void Pipeline_Executes_Null_Output_Processor()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();

            // Override the OnExecute to return a default ProcessorResult
            // which contains a null Output
            ((MockProcessor1)pipeline.Processors[1]).OnExecuteCalled =
                i => new ProcessorResult<string>() { };

            // The final pipeline result should reflect that error status
            ProcessorResult result = pipeline.Execute(new object[] { 5 });
            Assert.IsNotNull(result, "Failed to get a result");
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Expected successful status");
            Assert.IsNotNull(result.Output, "Pipeline should have set exit processor outputs");
            Assert.AreEqual(1, result.Output.Length, "Expected one output");
            Assert.IsNull(result.Output[0], "Expected exit processor's inputs never to have been set");
        }

        [TestMethod]
        [Description("Pipeline executing a processor that returns the wrong number of arguments throws")]
        public void Pipeline_Execute_Throws_Wrong_Output_Length()
        {
            MockPipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            ProcessorResult errorResult = null;

            // Override the OnExecute to return a ProcessorResult
            // which contains the wrong 
            ((MockProcessor1)pipeline.Processors[1]).OnExecuteCalled =
                i =>
                {
                    ProcessorResult pr = new ProcessorResult<string>() { };
                    pr.Output = new object[] { "string1", "string2" };
                    return pr as ProcessorResult<string>;
                };

            pipeline.OnErrorCalled = r => errorResult = r;

            // The final pipeline result should reflect that error status
            ProcessorResult result = pipeline.Execute(new object[] { 5 });
            Assert.IsNotNull(result, "Failed to get a result");
            Assert.AreEqual(ProcessorStatus.Error, result.Status, "Expected failure status");
            Assert.IsNotNull(result.Error, "Expected error result");
            Assert.AreEqual(typeof(InvalidOperationException), result.Error.GetType(), "Expected InvalidOperationException");
            
            Assert.IsNotNull(errorResult, "Pipeline.OnError should have been called");
        }

        [TestMethod]
        [Description("Pipeline executes a processor that throws in its execute")]
        public void Pipeline_Executes_Throwing_Processor()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            ProcessorResult errorResult = null;

            // Override the OnExecute to return a failure
            ((MockProcessor1)pipeline.Processors[1]).OnExecuteCalled = i =>
            {
                throw new InvalidOperationException("TestException");
            };

            // Override OnError to capture the error result
            ((MockProcessor1)pipeline.Processors[1]).OnErrorCalledAction = r => errorResult = r;

            // The final pipeline result should reflect that error status
            ProcessorResult result = pipeline.Execute(new object[] { 5 });
            Assert.IsNotNull(result, "Failed to get a result");
            Assert.AreEqual(ProcessorStatus.Error, result.Status, "Expected error status");

            Assert.IsNotNull(errorResult, "OnError should have been called");
        }

        #region execution scenarios
        [TestMethod]
        [Description("Pipeline executes without any argument bindings")]
        public void Pipeline_Executes_With_No_Bindings()
        {
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();
            MockPipeline pipeline = new MockPipeline(
                                        new Processor[] { processor1, processor2 },
                                        Enumerable.Empty<ProcessorArgument>(),
                                        Enumerable.Empty<ProcessorArgument>());
            pipeline.Initialize();
            ProcessorResult result = pipeline.Execute(new object[0]);
            Assert.IsNotNull(result, "Expected non-null ProcessorResult");
            Assert.IsNotNull(result.Output, "Expected non-null output from unbound pipeline");
            Assert.AreEqual(0, result.Output.Length, "Expected empty array from unbound pipeline");
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Expected OK status from pipeline");
        }

        [TestMethod]
        [Description("Pipeline executes without any argument bindings to the pipeline arguments")]
        public void Pipeline_Executes_With_No_Bindings_To_Pipeline()
        {
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();
            MockPipeline pipeline = new MockPipeline(
                                        new Processor[] { processor1, processor2 },
                                        Enumerable.Empty<ProcessorArgument>(),
                                        Enumerable.Empty<ProcessorArgument>());
            pipeline.BindArguments(processor1.OutArguments[0], processor2.InArguments[0]);
            pipeline.Initialize();
            ProcessorResult result = pipeline.Execute(new object[0]);
            Assert.IsNotNull(result, "Expected non-null ProcessorResult");
            Assert.IsNotNull(result.Output, "Expected non-null output from unbound pipeline");
            Assert.AreEqual(0, result.Output.Length, "Expected empty array from unbound pipeline");

            // Verify processor1 pushed default(int) through to processor2
            object value = pipeline.Context.ReadInput(processor2.InArguments[0]);
            Assert.IsNotNull(value);
            Assert.AreEqual(default(int).ToString(), value, "Expected default(int) to be passed from processor1 to processor2");
        }
        #endregion execution scenarios
    }
}
