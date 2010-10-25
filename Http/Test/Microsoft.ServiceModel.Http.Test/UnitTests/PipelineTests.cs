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
    public class PipelineTests
    {
        [TestMethod]
        [Description("Pipeline ctor throws if the processor collection is null")]
        public void Pipeline_Ctor_Throws_Null_Processors()
        {
            Pipeline p;
            ExceptionAssert.ThrowsArgumentNull(
                "Pipeline ctor should throw if processor collection is null",
                "processors",
                () => p = new Pipeline(null, Enumerable.Empty<ProcessorArgument>(), Enumerable.Empty<ProcessorArgument>())
                );
        }

        [TestMethod]
        [Description("Pipeline ctor throws if the input argument collection is null")]
        public void Pipeline_Ctor_Throws_Null_InArguments()
        {
            Processor processor = new MockProcessor1();
            Pipeline p;
            ExceptionAssert.ThrowsArgumentNull(
                "Pipeline ctor should throw if processor collection is null",
                "inArguments",
                () => p = new Pipeline(new[] { processor }, null, Enumerable.Empty<ProcessorArgument>())
                );
        }

        [TestMethod]
        [Description("Pipeline ctor throws if the output argument collection is null")]
        public void Pipeline_Ctor_Throws_Null_OutArguments()
        {
            Processor processor = new MockProcessor1();
            Pipeline p;
            ExceptionAssert.ThrowsArgumentNull(
                "Pipeline ctor should throw if processor collection is null",
                "outArguments",
                () => p = new Pipeline(new[] { processor }, Enumerable.Empty<ProcessorArgument>(), null)
                );
        }

        [TestMethod]
        [Description("A processor can belong to only one pipeline")]
        public void Pipeline_Processor_Belongs_To_Only_1_Pipeline()
        {
            Processor processor = new MockProcessor1();
            Pipeline pipeline1 = new Pipeline(new[] { processor }, Enumerable.Empty<ProcessorArgument>(), Enumerable.Empty<ProcessorArgument>());

            Assert.AreEqual(pipeline1, processor.ContainingCollection.ContainerProcessor, "Processor.Pipeline must be the pipeline we created");

            Pipeline pipeline2;
            ExceptionAssert.ThrowsInvalidOperation(
                "Adding a processor to a 2nd pipeline should throw",
                () => pipeline2 = new Pipeline(new[] { processor }, Enumerable.Empty<ProcessorArgument>(), Enumerable.Empty<ProcessorArgument>())
                );
        }

        #region binding

        [TestMethod]
        [Description("Pipeline BindArgument throws for null outArg")]
        public void Pipeline_BindArgument_Throws_Null_OutArg()
        {
            MockProcessor1 processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor }, 
                Enumerable.Empty<ProcessorArgument>(), 
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.ThrowsArgumentNull(
                "BindArgument with null out should throw",
                "outArgument",
                () => pipeline.BindArguments(null, processor.InArguments[0])
                );
        }

        [TestMethod]
        [Description("Pipeline BindArgument throws for null inArg")]
        public void Pipeline_BindArgument_Throws_Null_InArg()
        {
            MockProcessor1 processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.ThrowsArgumentNull(
                "BindArgument with null in should throw",
                "inArgument",
                () => pipeline.BindArguments(processor.OutArguments[0], null)
                );
        }

        [TestMethod]
        [Description("Pipeline BindArgument after initialize throws")]
        public void Pipeline_BindArgument_Throws_After_Initialize()
        {
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor1, processor2 },
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            pipeline.Initialize();

            ExceptionAssert.ThrowsInvalidOperation(
                "BindArgument should throw if called after initialize",
                () => pipeline.BindArguments(processor1.OutArguments[0], processor2.InArguments[0])
                );
        }

        [TestMethod]
        [Description("Pipeline UnbindArgument throws for null outArg")]
        public void Pipeline_UnbindArgument_Throws_Null_OutArg()
        {
            MockProcessor1 processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.ThrowsArgumentNull(
                "UnbindArgument with null out should throw",
                "outArgument",
                () => pipeline.UnbindArguments(null, processor.InArguments[0])
                );
        }

        [TestMethod]
        [Description("Pipeline UnbindArgument throws for null inArg")]
        public void Pipeline_UnbindArgument_Throws_Null_InArg()
        {
            MockProcessor1 processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.ThrowsArgumentNull(
                "UnbindArgument with null in should throw",
                "inArgument",
                () => pipeline.UnbindArguments(processor.OutArguments[0], null)
                );
        }

        [TestMethod]
        [Description("Pipeline UnbindArgument after initialize throws")]
        public void Pipeline_UnbindArgument_Throws_After_Initialize()
        {
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor1, processor2 },
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            pipeline.Initialize();

            ExceptionAssert.ThrowsInvalidOperation(
                "UnbindArgument should throw if called after initialize",
                () => pipeline.UnbindArguments(processor1.OutArguments[0], processor2.InArguments[0])
                );
        }


        [TestMethod]
        [Description("Pipeline GetBoundToArguments throws for null outArg")]
        public void Pipeline_GetBoundToArguments_Throws_Null_Parameer()
        {
            MockProcessor1 processor = new MockProcessor1();
            Pipeline pipeline = new Pipeline(
                Enumerable.Empty <Processor>(),
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.ThrowsArgumentNull(
                "GetBoundToArguments should throw for null parameter",
                "argument",
                () => pipeline.GetBoundToArguments(null)
                );
        }

        [TestMethod]
        [Description("Pipeline calls the OnBinding/OnBind/OnBoundArguments callbacks when bind")]
        public void Pipeline_Bind_OnBinding_OnBind_OnBound_Callbacks()
        {
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor1, processor2 }, 
                Enumerable.Empty<ProcessorArgument>(), 
                Enumerable.Empty<ProcessorArgument>());
            BindArgumentsEventArgs bindingEventArgs = null;
            BindArgumentsEventArgs boundEventArgs = null;
            ProcessorArgument inArgFromBind = null;
            ProcessorArgument outArgFromBind = null;

            pipeline.OnBindArgumentsCalled = (outArg, inArg) =>
            {
                inArgFromBind = inArg;
                outArgFromBind = outArg;
            };

            pipeline.BindingArguments = (s, a) => { bindingEventArgs = a as BindArgumentsEventArgs; };
            pipeline.BoundArguments = (s, a) => { boundEventArgs = a as BindArgumentsEventArgs; };

            pipeline.BindArguments(processor1.OutArguments[0], processor2.InArguments[0]);
            Assert.IsNotNull(bindingEventArgs, "Did not receive BindingArguments callback");
            Assert.AreSame(processor1.OutArguments[0], bindingEventArgs.OutArgument, "Did not receive correct outArg in BindingArgument callback");
            Assert.AreSame(processor2.InArguments[0], bindingEventArgs.InArgument, "Did not receive correct inArg in BindingArgument callback");

            Assert.IsNotNull(boundEventArgs, "Did not receive BoundArguments callback");
            Assert.AreSame(processor1.OutArguments[0], boundEventArgs.OutArgument, "Did not receive correct outArg in BoundArgument callback");
            Assert.AreSame(processor2.InArguments[0], boundEventArgs.InArgument, "Did not receive correct inArg in BoundArgument callback");

            Assert.AreSame(processor1.OutArguments[0], outArgFromBind, "Did not receive correct outArg in OnBind virtual");
            Assert.AreSame(processor2.InArguments[0], inArgFromBind, "Did not receive correct inArg in OnBind virtual");

        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineInput by index")]
        public void Pipeline_BindArgumentToPipelineInput_By_Index()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor }, 
                new ProcessorArgument[] { new ProcessorArgument("PipelineInput", typeof(int)) }, 
                Enumerable.Empty<ProcessorArgument>());

            BindArgumentsEventArgs eventArgs = null;
            ProcessorArgument inArgFromBind = null;
            ProcessorArgument outArgFromBind = null;

            pipeline.OnBindArgumentsCalled = (outArg, inArg) =>
                {
                    inArgFromBind = inArg;
                    outArgFromBind = outArg;
                };

            pipeline.BoundArguments = (s, a) => { eventArgs = a as BindArgumentsEventArgs; };
            pipeline.BindArgumentToPipelineInput(0, processor.InArguments[0]);
            Assert.IsNotNull(eventArgs, "Did not receive OnBoundArguments callback");
            Assert.AreSame(pipeline.Processors[0].OutArguments[0], outArgFromBind, "Did not receive correct outArg in OnBind virtual");
            Assert.AreSame(pipeline.Processors[1].InArguments[0], inArgFromBind, "Did not receive correct inArg in OnBind virtual");
            Assert.IsTrue(pipeline.GetBoundToArguments(pipeline.Processors[0].OutArguments[0]).Contains(processor.InArguments[0]),
                            "Failed to find processor 1's in argument in bindings for entry processor");
        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineInput by illegal index throws")]
        public void Pipeline_BindArgumentToPipelineInput_By_Bad_Index_Throws()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                new ProcessorArgument[] { new ProcessorArgument("PipelineInput", typeof(int)) },
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "BindArgumentToPipelineInput should throw with negative index",
                () => pipeline.BindArgumentToPipelineInput(-1, processor.InArguments[0]),
                "pipelineInputArgumentIndex"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "BindArgumentToPipelineInput should throw with too large an index",
                () => pipeline.BindArgumentToPipelineInput(1, processor.InArguments[0]),
                "pipelineInputArgumentIndex"
                );
        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineInput by name")]
        public void Pipeline_BindArgumentToPipelineInput_By_Name()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                new ProcessorArgument[] { new ProcessorArgument("PipelineInput", typeof(int)) },
                Enumerable.Empty<ProcessorArgument>());

            BindArgumentsEventArgs eventArgs = null;
            ProcessorArgument inArgFromBind = null;
            ProcessorArgument outArgFromBind = null;

            pipeline.OnBindArgumentsCalled = (outArg, inArg) =>
            {
                inArgFromBind = inArg;
                outArgFromBind = outArg;
            };

            pipeline.BoundArguments = (s, a) => { eventArgs = a as BindArgumentsEventArgs; };
            pipeline.BindArgumentToPipelineInput("PipelineInput", processor.InArguments[0]);
            Assert.IsNotNull(eventArgs, "Did not receive OnBoundArguments callback");
            Assert.AreSame(pipeline.Processors[0].OutArguments[0], outArgFromBind, "Did not receive correct outArg in OnBind virtual");
            Assert.AreSame(pipeline.Processors[1].InArguments[0], inArgFromBind, "Did not receive correct inArg in OnBind virtual");
            Assert.IsTrue(pipeline.GetBoundToArguments(pipeline.Processors[0].OutArguments[0]).Contains(processor.InArguments[0]),
                            "Failed to find processor 1's in argument in bindings for entry processor");
        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineInput by illegal name throws")]
        public void Pipeline_BindArgumentToPipelineInput_By_Bad_Name_Throws()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                new ProcessorArgument[] { new ProcessorArgument("PipelineInput", typeof(int)) },
                Enumerable.Empty<ProcessorArgument>());

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "BindArgumentToPipelineInput should throw with null name",
                () => pipeline.BindArgumentToPipelineInput(null, processor.InArguments[0]),
                "pipelineInputArgumentName"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "BindArgumentToPipelineInput should throw with empty name",
                () => pipeline.BindArgumentToPipelineInput(string.Empty, processor.InArguments[0]),
                "pipelineInputArgumentName"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "BindArgumentToPipelineInput should throw with whitespace name",
                () => pipeline.BindArgumentToPipelineInput("   ", processor.InArguments[0]),
                "pipelineInputArgumentName"
                );

            ExceptionAssert.ThrowsInvalidOperation(
                "BindArgumentToPipelineInput should throw with non-existent name",
                () => pipeline.BindArgumentToPipelineInput("NotAName", processor.InArguments[0])
                );
        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineOutput by Index")]
        public void Pipeline_BindArgumentToPipelineOutput_By_Index()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                new ProcessorArgument[] { new ProcessorArgument("PipelineOutput", typeof(string)) });

            BindArgumentsEventArgs eventArgs = null;
            ProcessorArgument inArgFromBind = null;
            ProcessorArgument outArgFromBind = null;

            pipeline.OnBindArgumentsCalled = (outArg, inArg) =>
            {
                inArgFromBind = inArg;
                outArgFromBind = outArg;
            };

            pipeline.BoundArguments = (s, a) => { eventArgs = a as BindArgumentsEventArgs; };
            pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], 0);
            Assert.IsNotNull(eventArgs, "Did not receive OnBoundArguments callback");
            Assert.AreSame(pipeline.Processors[1].OutArguments[0], outArgFromBind, "Did not receive correct outArg in OnBind virtual");
            Assert.AreSame(pipeline.Processors[2].InArguments[0], inArgFromBind, "Did not receive correct inArg in OnBind virtual");
            Assert.IsTrue(pipeline.GetBoundToArguments(pipeline.Processors[1].OutArguments[0]).Contains(pipeline.Processors[2].InArguments[0]),
                            "Failed to find exit processor in argument in bindings for processor 1");
        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineOutput by illegal index throws")]
        public void Pipeline_BindArgumentToPipelineOutput_By_Bad_Index_Throws()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                new ProcessorArgument[] { new ProcessorArgument("PipelineOutput", typeof(string)) });

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "BindArgumentToPipelineOutput should throw with negative index",
                () => pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], -1),
                "pipelineOutputArgumentIndex"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "BindArgumentToPipelineOutput should throw with too large an index",
                () => pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], 1),
                "pipelineOutputArgumentIndex"
                );
        }

        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineOutput by name")]
        public void Pipeline_BindArgumentToPipelineOutput_By_Name()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                new ProcessorArgument[] { new ProcessorArgument("PipelineOutput", typeof(string)) });

            BindArgumentsEventArgs eventArgs = null;
            ProcessorArgument inArgFromBind = null;
            ProcessorArgument outArgFromBind = null;

            pipeline.OnBindArgumentsCalled = (outArg, inArg) =>
            {
                inArgFromBind = inArg;
                outArgFromBind = outArg;
            };

            pipeline.BoundArguments = (s, a) => { eventArgs = a as BindArgumentsEventArgs; };
            pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], "PipelineOutput");
            Assert.IsNotNull(eventArgs, "Did not receive OnBoundArguments callback");
            Assert.AreSame(pipeline.Processors[1].OutArguments[0], outArgFromBind, "Did not receive correct outArg in OnBind virtual");
            Assert.AreSame(pipeline.Processors[2].InArguments[0], inArgFromBind, "Did not receive correct inArg in OnBind virtual");
            Assert.IsTrue(pipeline.GetBoundToArguments(pipeline.Processors[1].OutArguments[0]).Contains(pipeline.Processors[2].InArguments[0]),
                            "Failed to find exit processor in argument in bindings for processor 1");
        }


        [TestMethod]
        [Description("Pipeline BindArgumentToPipelineOutput by illegal name throws")]
        public void Pipeline_BindArgumentToPipelineOutput_By_Bad_Name_Throws()
        {
            MockProcessor1 processor = new MockProcessor1();
            MockPipeline pipeline = new MockPipeline(
                new Processor[] { processor },
                Enumerable.Empty<ProcessorArgument>(),
                new ProcessorArgument[] { new ProcessorArgument("PipelineOutput", typeof(string)) });

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "BindArgumentToPipelineOutput should throw with null name",
                () => pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], null),
                "pipelineOutputArgumentName"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "BindArgumentToPipelineOutput should throw with empty name",
                () => pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], string.Empty),
                "pipelineOutputArgumentName"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "BindArgumentToPipelineOutput should throw with whitespace name",
                () => pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], "   "),
                "pipelineOutputArgumentName"
                );

            ExceptionAssert.ThrowsInvalidOperation(
                "BindArgumentToPipelineInput should throw with non-existent name",
                () => pipeline.BindArgumentToPipelineOutput(processor.OutArguments[0], "NotAName")
                );
        }

        [TestMethod]
        [Description("Pipeline UnbindArgument succeeds")]
        public void Pipeline_UnbindArgument_Succeeds()
        {
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();
            Pipeline pipeline = new Pipeline(
                new Processor[] { processor1, processor2 },
                Enumerable.Empty<ProcessorArgument>(),
                Enumerable.Empty<ProcessorArgument>());

            pipeline.BindArguments(processor1.OutArguments[0], processor2.InArguments[0]);
            Assert.IsTrue(
                pipeline.GetBoundToArguments(processor1.OutArguments[0]).Contains(processor2.InArguments[0]),
                "Expected GetBoundTo(out) to show we bound out to in");
            Assert.IsTrue(
                pipeline.GetBoundToArguments(processor2.InArguments[0]).Contains(processor1.OutArguments[0]),
                "Expected GetBoundTo(in) to show we bound out to in");

            pipeline.UnbindArguments(processor1.OutArguments[0], processor2.InArguments[0]);

            Assert.IsFalse(
                pipeline.GetBoundToArguments(processor1.OutArguments[0]).Contains(processor2.InArguments[0]),
                "Expected GetBoundTo(out) to show we unbound out to in");
            Assert.IsFalse(
                pipeline.GetBoundToArguments(processor2.InArguments[0]).Contains(processor1.OutArguments[0]),
                "Expected GetBoundTo(in) to show we unbound out to in");
        }

        #endregion binding
    }
}
