// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PipelineContextInfoTests
    {
        [TestMethod]
        [Description("PipelineContextInfo ctor initializes correctly")]
        public void PipelineContextInfo_Ctor_Initializes()
        {
            Pipeline pipeline = TestPipelines.CreateMockProcessor1Pipeline();
            PipelineContextInfo info = new PipelineContextInfo(pipeline);

            Assert.AreEqual(pipeline, info.Pipeline, "Pipeline property incorrect");
            Assert.AreEqual(2, info.TotalInputValueCount, "Context should have discovered 2 inputs");

            // Entry processor should have zero inputs
            int inputValueOffset;
            int inputValueCount = info.GetInputValueInfo(0, out inputValueOffset);
            Assert.AreEqual(0, inputValueCount, "Entry processor should have no inputs");
            Assert.AreEqual(inputValueCount, pipeline.Processors[0].InArguments.Count, "Incorrect in arg count for processor 0");

            // MockProcessor1 should have 1 input
            inputValueCount = info.GetInputValueInfo(1, out inputValueOffset);
            Assert.AreEqual(1, inputValueCount, "MockProcessor1 should have 1 input");
            Assert.AreEqual(inputValueCount, pipeline.Processors[1].InArguments.Count, "Incorrect in arg count for processor 1");

            // Exit processor should have 1 input
            inputValueCount = info.GetInputValueInfo(2, out inputValueOffset);
            Assert.AreEqual(1, inputValueCount, "Exit processor should have 1 input");
            Assert.AreEqual(inputValueCount, pipeline.Processors[2].InArguments.Count, "Incorrect in arg count for processor 2");

            // 
            // Entry processor should have 1 out arg that maps to 1 in arg in processor 1
            //
            ProcessorArgument outArg;
            ProcessorArgument[] inArgs;
            int[] inputValueIndices = info.GetOutputValueInfo(0, 0, out outArg, out inArgs);

            // That outArg should be "IntValue" and belong to the entry processor
            Assert.IsNotNull(outArg, "OutputArgument was not returned");
            Assert.AreEqual("intValue", outArg.Name, "Processor[0] out arg should have been intValue");
            Assert.AreEqual(pipeline.Processors[0], outArg.ContainingCollection.Processor, "OutArg for processor 0 should belong to processor 0");
            
            // That outArg should map to a single inArg in processor 1
            Assert.IsNotNull(inArgs, "InputArg array was not returned");
            Assert.IsNotNull(inputValueIndices, "Input value indices were not returned");

            Assert.AreEqual(inArgs.Length, inputValueIndices.Length, "Input args and their indices arrays should match in length");
            Assert.AreEqual(1, inArgs.Length, "Should have found 1 input arg mapped to processor 1's output");
            Assert.AreEqual("intValue", inArgs[0].Name, "Entry processor output should have mapped to intValue input");
            Assert.AreEqual(pipeline.Processors[1], inArgs[0].ContainingCollection.Processor, "Entry processor output maps to inArg of wrong processor");

            //
            // Processor 1 should have 1 out arg that maps to in arg in exit processor
            //
            info.GetOutputValueInfo(1, 0, out outArg, out inArgs);

            // That outArg should be "theResult" and belong to processor 1
            Assert.IsNotNull(outArg, "OutputArgument was not returned");
            Assert.AreEqual("MockProcessor1Result", outArg.Name, "Processor 1 out arg should have been MockProcessor1Result");
            Assert.AreEqual(pipeline.Processors[1], outArg.ContainingCollection.Processor, "OutArg for processor 1 should belong to processor 1");

            // That outArg should map to a single inArg from processor 2
            Assert.IsNotNull(inArgs, "InputArg array was not returned");
            Assert.IsNotNull(inputValueIndices, "Input value indices were not returned");

            Assert.AreEqual(inArgs.Length, inputValueIndices.Length, "Input args and their indices arrays should match in length");
            Assert.AreEqual(1, inArgs.Length, "Should have found 1 input arg mapped to entry processor's output");
            Assert.AreEqual("theResult", inArgs[0].Name, "Entry processor output should have mapped to intValue input");
            Assert.AreEqual(pipeline.Processors[2], inArgs[0].ContainingCollection.Processor, "processor 1 output maps to inArg of wrong processor");

        }
    }
}
