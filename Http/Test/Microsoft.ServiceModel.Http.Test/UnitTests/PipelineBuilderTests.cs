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
    public class PipelineBuilderTests
    {
        #region virtual methods
        [TestMethod]
        [Description("PipelineBuilder OnFilter virtual throws for bad parameters")]
        public void PipelineBuilder_OnFilter_Throws_Bad_Parameters()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            IEnumerable<Processor> emptyProcessors = Enumerable.Empty<Processor>();
            IEnumerable<ProcessorArgument> emptyArgs = Enumerable.Empty<ProcessorArgument>();

            // Returns null from OnFilter virtual causes throw
            pb.OnFilterCalled = p => null;

            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder.OnFilter should throw for null processes",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs) 
                );

            pb = new MockPipelineBuilder();
            pb.OnFilterProcessors = p => null;

            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnFilter should throw for null processes passed to base",
                "processors",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );
        }

        [TestMethod]
        [Description("PipelineBuilder OnOrder virtual throws for bad parameters")]
        public void PipelineBuilder_OnOrder_Throws_Bad_Parameters()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            IEnumerable<Processor> emptyProcessors = Enumerable.Empty<Processor>();
            IEnumerable<ProcessorArgument> emptyArgs = Enumerable.Empty<ProcessorArgument>();

            // Returns null from OnOrder virtual
            pb.OnOrderCalled = p => null;

            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder.OnOrder should throw for null processes",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );
        }

        [TestMethod]
        [Description("PipelineBuilder OnInitialize virtual throws for bad parameters")]
        public void PipelineBuilder_OnInitialize_Throws_Bad_Parameters()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            IEnumerable<Processor> emptyProcessors = Enumerable.Empty<Processor>();
            IEnumerable<ProcessorArgument> emptyArgs = Enumerable.Empty<ProcessorArgument>();

            // Returns null from OnInitialize virtual
            pb.OnInitializeCalled = p => null;

            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnInitialize should throw for null processes",
                "pipeline",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );
        }

        [TestMethod]
        [Description("PipelineBuilder OnCreatePipeline virtual throws for bad parameters")]
        public void PipelineBuilder_OnCreatePipeline_Throws_Bad_Parameters()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            IEnumerable<Processor> emptyProcessors = Enumerable.Empty<Processor>();
            IEnumerable<ProcessorArgument> emptyArgs = Enumerable.Empty<ProcessorArgument>();

            // Returns null from OnInitialize virtual
            pb.OnCreatePipelineCalled = (p,inArgs,outArgs) => null;

            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder.OnCreatePipeline should throw for null processes",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );

            pb = new MockPipelineBuilder();
            pb.OnCreatePipelineProcessors = p => null;
            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnCreatePipeline should throw for null processes argument",
                "processors",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );

            pb = new MockPipelineBuilder();
            pb.OnCreatePipelineInArgs = p => null;
            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnCreatePipeline should throw for null inArgs argument",
                "inArguments",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );

            pb = new MockPipelineBuilder();
            pb.OnCreatePipelineOutArgs = p => null;
            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnCreatePipeline should throw for null outArgs argument",
                "outArguments",
                () => pb.Build(emptyProcessors, emptyArgs, emptyArgs)
                );
        }

        [TestMethod]
        [Description("PipelineBuilder OnShouldBind virtual throws for bad parameters")]
        public void PipelineBuilder_OnShouldBind_Throws_Bad_Parameters()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            Processor[] processors = new Processor[] { new MockProcessor1(), new MockProcessorEchoString() };
            IEnumerable<ProcessorArgument> emptyArgs = Enumerable.Empty<ProcessorArgument>();

            // Pushes null into outArg in OnShouldBind virtual
            pb.OnShouldArgumentBindOutArg = a => null;

            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnShouldBind should throw for null outArg",
                "outArgument",
                () => pb.Build(processors, emptyArgs, emptyArgs)
                );

            // Get a new pipeline because the prior one is now in a damaged state
            pb = new MockPipelineBuilder();
            processors = new Processor[] { new MockProcessor1(), new MockProcessorEchoString() };

            // Pushes null into outArg in OnShouldBind virtual
            pb.OnShouldArgumentBindInArg = a => null; 

            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnShouldBind should throw for null inArg",
                "inArgument",
                () => pb.Build(processors, emptyArgs, emptyArgs)
                );
        }


        [TestMethod]
        [Description("PipelineBuilder OnGetRelativeExecutionOrder virtual throws for bad parameters")]
        public void PipelineBuilder_OnGetRelativeExecutionOrder_Throws_Bad_Parameters()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            Processor[] processors = new Processor[] { new MockProcessor1(), new MockProcessorEchoString() };
            IEnumerable<ProcessorArgument> emptyArgs = Enumerable.Empty<ProcessorArgument>();

            // Pushes null into firstProcessor in OnGetRelativeExecutionOrder virtual
            pb.OnGetRelativeExecutionOrderFirstProcessor = a => null;

            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnGetRelativeExecutionOrder should throw for null first processor",
                "firstProcessor",
                () => pb.Build(processors, emptyArgs, emptyArgs)
                );

            // Get a new pipeline because the prior one is now in a damaged state
            pb = new MockPipelineBuilder();
            processors = new Processor[] { new MockProcessor1(), new MockProcessorEchoString() };

            pb.OnGetRelativeExecutionOrderSecondProcessor = a => null;

            ExceptionAssert.ThrowsArgumentNull(
                "PipelineBuilder.OnGetRelativeExecutionOrder should throw for null second processor",
                "secondProcessor",
                () => pb.Build(processors, emptyArgs, emptyArgs)
                );
        }


        #endregion virtual methods

        #region derived pipelinebuilders

        [TestMethod]
        [Description("PipelineBuilder for derived pipeline works")]
        public void PipelineBuilder_Derived_Pipeline()
        {
            MockPipelineBuilderOfMockPipeline pb = new MockPipelineBuilderOfMockPipeline();
            Processor[] processors = new Processor[0];
            ProcessorArgument[] inArgs = new ProcessorArgument[0];
            ProcessorArgument[] outArgs = new ProcessorArgument[0];
            MockPipeline pipeline = pb.Build(processors, inArgs, outArgs);
        }

        [TestMethod]
        [Description("PipelineBuilder for derived pipeline throws if no appropriate create is provided")]
        public void PipelineBuilder_Derived_Pipeline_No_Create()
        {
            MockPipelineBuilderOfMockPipelineNoCreate pb = new MockPipelineBuilderOfMockPipelineNoCreate();
            Processor[] processors = new Processor[0];
            ProcessorArgument[] inArgs = new ProcessorArgument[0];
            ProcessorArgument[] outArgs = new ProcessorArgument[0];
            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder without appropriate OnCreate throws",
                () => pb.Build(processors, inArgs, outArgs)
                );
        }

        #endregion derived pipelinebuilders

        #region build scenarios
        [TestMethod]
        [Description("PipelineBuilder Build succeeds with no processors")]
        public void PipelineBuilder_Builds_No_Processors()
        {
            PipelineBuilder pb = new PipelineBuilder();
            Pipeline pipeline = pb.Build(
                                    Enumerable.Empty<Processor>(), 
                                    Enumerable.Empty<ProcessorArgument>(),
                                    Enumerable.Empty<ProcessorArgument>());
            Assert.IsNotNull(pipeline);
        }

        [TestMethod]
        [Description("PipelineBuilder Build throws if null processors")]
        public void PipelineBuilder_Throws_Null_Processors()
        {
            PipelineBuilder pb = new PipelineBuilder();
            ExceptionAssert.ThrowsArgumentNull(
                "Build should throw if null processors",
                "processors",
                () => pb.Build(
                            null,
                            Enumerable.Empty<ProcessorArgument>(),
                            Enumerable.Empty<ProcessorArgument>())
            );
        }

        [TestMethod]
        [Description("PipelineBuilder Build throws if null in arguments")]
        public void PipelineBuilder_Throws_Null_InArguments()
        {
            PipelineBuilder pb = new PipelineBuilder();
            ExceptionAssert.ThrowsArgumentNull(
                "Build should throw if null in arguments",
                "inArguments",
                () => pb.Build(
                            Enumerable.Empty<Processor>(),
                            null,
                            Enumerable.Empty<ProcessorArgument>())
            );
        }

        [TestMethod]
        [Description("PipelineBuilder Build throws if null out arguments")]
        public void PipelineBuilder_Throws_Null_OutArguments()
        {
            PipelineBuilder pb = new PipelineBuilder();
            ExceptionAssert.ThrowsArgumentNull(
                "Build should throw if null out arguments",
                "outArguments",
                () => pb.Build(
                            Enumerable.Empty<Processor>(),
                            Enumerable.Empty<ProcessorArgument>(),
                            null)
            );
        }

        [TestMethod]
        [Description("PipelineBuilder Build throws if virtual OnFilter returns null")]
        public void PipelineBuilder_Build_Throws_If_OnFilter_Returns_Null()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            pb.OnFilterCalled = p => null;
            ExceptionAssert.ThrowsInvalidOperation(
                "Build should throw if OnFilter returns null",
                () => pb.Build(
                            Enumerable.Empty<Processor>(),
                            Enumerable.Empty<ProcessorArgument>(),
                            Enumerable.Empty<ProcessorArgument>())
                );
        }

        [TestMethod]
        [Description("PipelineBuilder Build throws if virtual OnOrder returns null")]
        public void PipelineBuilder_Build_Throws_If_OnOrder_Returns_Null()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            pb.OnOrderCalled = p => null;
            ExceptionAssert.ThrowsInvalidOperation(
                "Build should throw if OnOrder returns null",
                () => pb.Build(
                            Enumerable.Empty<Processor>(),
                            Enumerable.Empty<ProcessorArgument>(),
                            Enumerable.Empty<ProcessorArgument>())
                );
        }

        [TestMethod]
        [Description("PipelineBuilder Build binds correctly by name")]
        public void PipelineBuilder_Build_Binds_By_Name()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();

            // Rename processor inputs and outputs to bind by name to pipeline arguments and to each other:
            // pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p2[stringOutput] --> [stringOutput]pipeline
            processor1.InArguments[0].Name = "intInput"; 
            processor2.InArguments[0].Name = processor1.OutArguments[0].Name = "echoInput";
            processor2.OutArguments[0].Name = "stringOutput";

            Pipeline pipeline = pb.Build(
                new Processor[] { processor1, processor2 },
                new ProcessorArgument[] { new ProcessorArgument("intInput", typeof(int)) },
                new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
            );

            Assert.IsNotNull(pipeline, "Build should produce non-null pipeline");

            ProcessorArgument[] boundArgs = pipeline.GetBoundToArguments(pipeline.Processors[0].OutArguments[0]).ToArray();
            Assert.AreEqual(1, boundArgs.Length, "Expected 1 parameter bound to entry processor output");
            Assert.AreSame(processor1.InArguments[0], boundArgs[0], "Expected entry processor output to bind to processor 1 input");

            boundArgs = pipeline.GetBoundToArguments(pipeline.Processors[1].OutArguments[0]).ToArray();
            Assert.AreEqual(1, boundArgs.Length, "Expected 1 parameter bound to first processor output");
            Assert.AreSame(processor2.InArguments[0], boundArgs[0], "Expected first processor output to bind to second processor input");

            boundArgs = pipeline.GetBoundToArguments(pipeline.Processors[2].OutArguments[0]).ToArray();
            Assert.AreEqual(1, boundArgs.Length, "Expected 1 parameter bound to second processor output");
            Assert.AreSame(pipeline.Processors[3].InArguments[0], boundArgs[0], "Expected second processor output to bind to exit processor input");
        }

        [TestMethod]
        [Description("PipelineBuilder Build invokes all the virtual")]
        public void PipelineBuilder_Build_Invokes_All_Virtuals()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            bool onOrderCalled = false;
            bool onFilterCalled = false;
            bool onCreatePipelineCalled = false;
            bool onInitializeCalled = false;
            bool onValidateCalled = false;
            bool onShouldBindCalled = false;

            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();

            pb.OnFilterCalled = p =>
            {
                onFilterCalled = true;
                Assert.IsFalse(onOrderCalled || onInitializeCalled || onCreatePipelineCalled || onValidateCalled || onShouldBindCalled,
                         "OnFilter should be called before OnOrder, OnInitialize, OnCreatePipeline, OnValidate, and OnShouldBind");
                return p;
            };

            pb.OnOrderCalled = p =>
            {
                onOrderCalled = true;
                Assert.IsTrue(onFilterCalled, "OnOrder should be called after OnFilter");
                Assert.IsFalse(onInitializeCalled || onCreatePipelineCalled || onValidateCalled || onShouldBindCalled,
                                "OnOrder should be called before OnInitialize, OnCreatePipeline, OnValidate, and OnShouldBind");       
                return p;
            };

            pb.OnCreatePipelineCalled = (p, aIn, aOut) =>
            {
                onCreatePipelineCalled = true;
                Assert.IsTrue(onFilterCalled && onOrderCalled, "OnCreatePipeline should be called after OnFilter and OnOrder");
                Assert.IsFalse(onInitializeCalled || onValidateCalled || onShouldBindCalled,
                                "OnCreatePipeline should be called before OnInitialize, OnValidate, and OnShouldBind");
                return new Pipeline(p, aIn, aOut);
            };

            pb.OnInitializeCalled = p =>
            {
                onInitializeCalled = true;
                Assert.IsNotNull(p, "Oninitialize passed null pipeline");
                Assert.IsTrue(onFilterCalled && onOrderCalled && onCreatePipelineCalled, "OnInitialize should be called after OnFilter, OnCreatePipeline and OnOrder");
                Assert.IsFalse(onValidateCalled || onShouldBindCalled,
                                 "OnInitialize should be called before OnValidate and OnShouldBind");
                return p;
            };

            pb.OnShouldArgumentBindCalled = (aOut, aIn) =>
            {
                onShouldBindCalled = true;
                Assert.IsTrue(onFilterCalled && onOrderCalled && onInitializeCalled && onCreatePipelineCalled, "OnInitialize should be called after OnFilter, OnCreatePipeline, OnOrder and OnInitialize");
                Assert.IsFalse(onValidateCalled, "OnShouldArgmentBind should be called before OnValidate");
                return null;
            };

            pb.OnValidateCalled = p =>
            {
                Assert.IsTrue(onFilterCalled && onOrderCalled && onInitializeCalled && onShouldBindCalled && onCreatePipelineCalled,
                                "OnInitialize should be called after OnFilter, OnOrder, OnInitialize, OnCreatePipeline and OnShouldBind");
                onValidateCalled = true;
                return true;    // call base.validate
            };

            // Rename processor inputs and outputs to bind by name to pipeline arguments and to each other:
            // pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p2[stringOutput] --> [stringOutput]pipeline
            processor1.InArguments[0].Name = "intInput";        // same as pipeline input
            processor2.InArguments[0].Name = processor1.OutArguments[0].Name = "echoInput";
            processor2.OutArguments[0].Name = "stringOutput";   // same as pipeline output

            Pipeline pipeline = pb.Build(
                new Processor[] { processor1, processor2 },
                new ProcessorArgument[] { new ProcessorArgument("intInput", typeof(int)) },
                new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
            );

            Assert.IsNotNull(pipeline, "Build should produce non-null pipeline");

            Assert.IsTrue(onFilterCalled, "OnFilter was not called");
            Assert.IsTrue(onOrderCalled, "OnOrder was not called");
            Assert.IsTrue(onInitializeCalled, "OnInitialize was not called");
            Assert.IsTrue(onShouldBindCalled, "OnShouldBind was not called");
            Assert.IsTrue(onValidateCalled, "OnValidate was not called");
        }

        [TestMethod]
        [Description("PipelineBuilder Build binds correctly by name and executes")]
        public void PipelineBuilder_Build_Binds_By_Name_And_Executes()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();

            // Rename processor inputs and outputs to bind by name to pipeline arguments and to each other:
            // pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p2[stringOutput] --> [stringOutput]pipeline
            processor1.InArguments[0].Name = "intInput";
            processor2.InArguments[0].Name = processor1.OutArguments[0].Name = "echoInput";
            processor2.OutArguments[0].Name = "stringOutput";

            Pipeline pipeline = pb.Build(
                new Processor[] { processor1, processor2 },
                new ProcessorArgument[] { new ProcessorArgument("intInput", typeof(int)) },
                new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
            );

            Assert.IsNotNull(pipeline, "Build should produce non-null pipeline");

            ProcessorResult result = pipeline.Execute(new object[] { 9 });
            Assert.IsNotNull(result, "Null result from execute");
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Expected OK result status");
            Assert.IsNotNull(result.Output, "Expected non-null output");
            Assert.AreEqual(1, result.Output.Length, "Expected 1 output value");
            Assert.AreEqual(9.ToString(), result.Output[0], "Wrong output value");
        }

        [TestMethod]
        [Description("PipelineBuilder Build correctly removes correct conditional processors")]
        public void PipelineBuilder_Build_Removes_Filtered_Processors()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            MockProcessor1 processor1 = new MockProcessor1();
            ConditionalMockProcessorEchoString processor2 = new ConditionalMockProcessorEchoString() { WillExecute = true };
            ConditionalMockProcessorEchoString processor3 = new ConditionalMockProcessorEchoString() { WillExecute = false };

            // Rename processor inputs and outputs to bind by name to pipeline arguments and to each other:
            // pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p2[stringOutput] --> [stringOutput]pipeline
            // Also temporarily, have this before it gets yanked:
            //    pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p3[stringOutput2]
            processor1.InArguments[0].Name = "intInput";
            processor3.InArguments[0].Name = processor2.InArguments[0].Name = processor1.OutArguments[0].Name = "echoInput";
            processor2.OutArguments[0].Name = "stringOutput";
            processor3.OutArguments[0].Name = "stringOutput2";

            Pipeline pipeline = pb.Build(
                new Processor[] { processor1, processor2, processor3 },
                new ProcessorArgument[] { new ProcessorArgument("intInput", typeof(int)) },
                new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
            );

            Assert.IsNotNull(pipeline, "Build should produce non-null pipeline");
            Assert.AreEqual(4, pipeline.Processors.Count, "Should have filtered to 4 processors");
            Assert.IsFalse(pipeline.Processors.Contains(processor3), "Processor3 should have been filtered out");
        }

        [TestMethod]
        [Description("PipelineBuilder Build orders processors correctly")]
        public void PipelineBuilder_Build_Orders_Processors()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            MockProcessor1 processor1 = new MockProcessor1() { Name = "Processor1" };
            OrderableMockProcessorEchoString processor2 = new OrderableMockProcessorEchoString() { Name = "Processor2" };
            OrderableMockProcessorEchoString processor3 = new OrderableMockProcessorEchoString() { Name = "Processor3" };

            // Only P2 provides an answer -- all the rest must adjust to that
            processor2.RunAfter = processor3;

            // Rename processor inputs and outputs to bind by name to pipeline arguments and to each other:
            // pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p2[stringOutput] --> [stringOutput]pipeline
            //                        [intInput]p1[echoInput] --> [echoInput]p3[stringOutput2]
            processor1.InArguments[0].Name = "intInput";
            processor3.InArguments[0].Name = processor2.InArguments[0].Name = processor1.OutArguments[0].Name = "echoInput";
            processor2.OutArguments[0].Name = "stringOutput";
            processor3.OutArguments[0].Name = "stringOutput2";

            Pipeline pipeline = pb.Build(
                new Processor[] { processor1, processor2, processor3 },
                new ProcessorArgument[] { new ProcessorArgument("intInput", typeof(int)) },
                new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
            );

            Assert.IsNotNull(pipeline, "Build should produce non-null pipeline");
            Assert.AreEqual(5, pipeline.Processors.Count, "Should have 5 processors");
            int index2 = pipeline.Processors.IndexOf(processor2);
            int index3 = pipeline.Processors.IndexOf(processor3);
            Assert.IsTrue(index2 > index3, "Processor2 should have been ordered after processor3");
        }

        #endregion build scenario

        #region ordering scenarios

        [TestMethod]
        [Description("PipelineBuilder Build orders processors correctly when A is first and adjacent")]
        public void PipelineBuilder_Build_Orders_Processors_AFirst_Adjacent()
        {
            this.AssertOrderingCorrect(startWithABeforeB: true, aAndBadjacent: true);
        }

        [TestMethod]
        [Description("PipelineBuilder Build orders processors correctly when B is first and adjacent")]
        public void PipelineBuilder_Build_Orders_Processors_BFirst_Adjacent()
        {
            this.AssertOrderingCorrect(startWithABeforeB: false, aAndBadjacent: true);
        }

        [TestMethod]
        [Description("PipelineBuilder Build orders processors correctly when A is first and not adjacent")]
        public void PipelineBuilder_Build_Orders_Processors_AFirst_Not_Adjacent()
        {
            this.AssertOrderingCorrect(startWithABeforeB: true, aAndBadjacent: false);
        }

        [TestMethod]
        [Description("PipelineBuilder Build orders processors correctly when B is first and not adjacent")]
        public void PipelineBuilder_Build_Orders_Processors_BFirst_Not_Adjacent()
        {
            this.AssertOrderingCorrect(startWithABeforeB: false, aAndBadjacent: false);
        }


        [TestMethod]
        [Description("PipelineBuilder OnGetRelativeExecutionOrder virtual is called during ordering")]
        public void PipelineBuilder_OnGetRelativeExecutionOrder_Virtual_IsCalled()
        {
            Processor processor1 = new MockProcessor1();
            Processor processor2 = new MockProcessorEchoString();
            Processor processor3 = new MockProcessorNoArgs();

            Processor[] processors = new Processor[] { processor1, processor2, processor3 };
            ProcessorArgument[] inArgs = new[] { new ProcessorArgument("intValue", typeof(int)) };
            ProcessorArgument[] outArgs = new ProcessorArgument[0];

            // Bind input of echo processor to mock proc 1
            processor2.InArguments[0].Name = processor1.OutArguments[0].Name;

            bool firstOrderSet = false;
            bool secondOrderSet = false;
            bool thirdOrderSet = false;
            ProcessorExecutionOrder firstOrder = default(ProcessorExecutionOrder);
            ProcessorExecutionOrder secondOrder = default(ProcessorExecutionOrder);
            ProcessorExecutionOrder thirdOrder = default(ProcessorExecutionOrder);

            // Get a new pipeline because the prior one is now in a damaged state
            MockPipelineBuilder pb = new MockPipelineBuilder();

            pb.OnGetRelativeExecutionOrderCalled = (p1, p2, order) =>
                {
                    if (p1 == processors[0] && p2 == processors[1])
                    {
                        firstOrder = order;
                        firstOrderSet = true;
                    }

                    if (p1 == processors[1] && p2 == processors[0])
                    {
                        secondOrder = order;
                        secondOrderSet = true;
                    }

                    if (p1 == processors[2] && p2 == processors[0])
                    {
                        thirdOrder = order;
                        thirdOrderSet = true;
                    }
                    return order;
                };

            Pipeline pipeline = pb.Build(processors, inArgs, outArgs);

            Assert.IsTrue(firstOrderSet, "OnGetRelativeExecutionOrder virtual was not called for (p1,p2)");
            Assert.IsTrue(secondOrderSet, "OnGetRelativeExecutionOrder virtual was not called for (p2,p1)");
            Assert.IsTrue(thirdOrderSet, "OnGetRelativeExecutionOrder virtual was not called for (p3,p1)");

            Assert.AreEqual(ProcessorExecutionOrder.Before, firstOrder, "The (p1,p2) call should have yielded 'before'");
            Assert.AreEqual(ProcessorExecutionOrder.After, secondOrder, "The (p2, p1) call should have yielded 'after'");
            Assert.AreEqual(ProcessorExecutionOrder.Impartial, thirdOrder, "The (p3, p1) call should have yielded 'impartial'");
        }

        [TestMethod]
        [Description("PipelineBuilder Ordering random tests")]
        public void PipelineBuilder_Ordering_Random()
        {
            Random random = new Random();
            int seed = random.Next();

            try
            {
                for (int i = 0; i < 20; ++i)
                {
                    this.PipelineBuilder_Ordering_Random(random);
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Starting random seed for this failing pass was: " + seed);
                throw;
            }
        }

        private void PipelineBuilder_Ordering_Random(Random random)
        {
            int nProcessors = (random.Next() % 10) + 1;

            System.Diagnostics.Debug.WriteLine("Number processors this pass: " + nProcessors);

            Processor[] processors = new Processor[nProcessors];
            for (int i = 0; i < nProcessors; ++i)
            {
                MockProcessorEchoString p = new MockProcessorEchoString() { Name = "Processor" + (i + 1) };
                p.OutArguments[0].Name = p.Name + "Out";
                processors[i] = p;
            }

            for (int i = 0; i < nProcessors; ++i)
            {
                if (i == 0)
                {
                    processors[i].InArguments[0].Name = "PipeIn";
                }
                else
                {
                    processors[i].InArguments[0].Name = processors[i - 1].OutArguments[0].Name;
                }
            }

            Processor[] origProcessors = new Processor[nProcessors];
            Array.Copy(processors, origProcessors, nProcessors);

            // Now randomly scramble the processor order
            for (int i = 0; i < nProcessors; ++i)
            {
                int iSrc = random.Next() % nProcessors;
                int iDst = random.Next() % nProcessors;

                var temp = processors[iSrc];
                processors[iSrc] = processors[iDst];
                processors[iDst] = temp;
            }

            MockPipelineBuilder builder = new MockPipelineBuilder();

            Pipeline pipeline = builder.Build(
                                    processors,
                                    new ProcessorArgument[] { new ProcessorArgument("PipeIn", typeof(string)) },
                                    new ProcessorArgument[0]
                );

            for (int i = 0; i < nProcessors; ++i)
            {
                Assert.AreSame(origProcessors[i], pipeline.Processors[i + 1],
                    "Slot " + i + " had " + ((MockProcessorEchoString)pipeline.Processors[i + 1]).Name +
                    " but expected " + ((MockProcessorEchoString)origProcessors[i]).Name);
            }

        }


        #endregion ordering scenarios

        #region validation scenarios


        [TestMethod]
        [Description("PipelineBuilder Validate accepts empty pipeline with input bound to output")]
        public void PipelineBuilder_Validate_Accepts_Empty_With_Bound_Input_Output()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            Pipeline pipeline = pb.Build(
                    Enumerable.Empty<Processor>(),
                    new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) },
                    new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
                    );
            Assert.IsNotNull(pipeline, "Expected pipeline output");
        }

        [TestMethod]
        [Description("PipelineBuilder Validate throws if required pipeline output is unbound")]
        public void PipelineBuilder_Validate_Throws_Unbound_Pipeline_Output()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder should throw when pipeline output is unbound",
                () => pb.Build(
                    Enumerable.Empty<Processor>(),
                    Enumerable.Empty<ProcessorArgument>(),
                    new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
                    )
            );
        }

        [TestMethod]
        [Description("PipelineBuilder Validate throws if required pipeline output differs in type from input")]
        public void PipelineBuilder_Validate_Throws_Wrong_Type_Bind_Pipeline_Output()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder should throw when pipeline output binds to wrong type",
                () => pb.Build(
                    Enumerable.Empty<Processor>(),
                    new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(double)) },
                    new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
                    )
            );
        }


        [TestMethod]
        [Description("PipelineBuilder Validate throws if required input occurs before output")]
        public void PipelineBuilder_Validate_Throws_Input_Before_Output()
        {
            MockPipelineBuilder pb = new MockPipelineBuilder();
            MockProcessor1 processor1 = new MockProcessor1();
            MockProcessorEchoString processor2 = new MockProcessorEchoString();

            // Rename processor inputs and outputs to bind by name to pipeline arguments and to each other:
            // pipeline[intInput] --> [intInput]p1[echoInput] --> [echoInput]p2[stringOutput] --> [stringOutput]pipeline
            processor1.InArguments[0].Name = "intInput";
            processor2.InArguments[0].Name = processor1.OutArguments[0].Name = "echoInput";
            processor2.OutArguments[0].Name = "stringOutput";

            // Defeat normal ordering
            pb.OnOrderCalled = p => p;

            ExceptionAssert.ThrowsInvalidOperation(
                "PipelineBuilder should throw when required input occurs before output",
                () =>  pb.Build(
                new Processor[] { processor2, processor1 }, // <<<<<<<<< notice the order is flipped
                new ProcessorArgument[] { new ProcessorArgument("intInput", typeof(int)) },
                new ProcessorArgument[] { new ProcessorArgument("stringOutput", typeof(string)) }
                )
            );
        }
        #endregion validation scenarios

        #region local mocks

        public class MockPipelineBuilderOfMockPipelineNoCreate : PipelineBuilder<MockPipeline>
        {
        }

        public class ConditionalMockProcessorEchoString : MockProcessorEchoString, IConditionalExecutionProcessor
        {
            public bool WillExecute { get; set; }
        }

        public class OrderableMockProcessorEchoString : MockProcessorEchoString, IOrderableProcessor
        {
            public Processor RunBefore { get; set; }
            public Processor RunAfter { get; set; }
            public ProcessorExecutionOrder GetRelativeExecutionOrder(Processor processor)
            {
                if (processor == this.RunBefore)
                {
                    return ProcessorExecutionOrder.Before;
                }
                
                if (processor == this.RunAfter)
                {
                    return ProcessorExecutionOrder.After;
                }
                
                return ProcessorExecutionOrder.Impartial;
            }
        }

        #endregion local mocks

        #region test helpers

        private object[][] truthTable = new object[][] {
                // Means:         if A says this,             and B says this,     0=n/c, 1=swap, -1=throw
                new object[] { ProcessorExecutionOrder.Impartial, ProcessorExecutionOrder.Impartial,     0 },
                new object[] { ProcessorExecutionOrder.Before,    ProcessorExecutionOrder.Impartial,     0 },
                new object[] { ProcessorExecutionOrder.After,     ProcessorExecutionOrder.Impartial,     1 },
                new object[] { ProcessorExecutionOrder.Before,    ProcessorExecutionOrder.After,         0 },
                new object[] { ProcessorExecutionOrder.Before,    ProcessorExecutionOrder.Before,       -1 },
                new object[] { ProcessorExecutionOrder.After,     ProcessorExecutionOrder.Before,        1 },
                new object[] { ProcessorExecutionOrder.After,     ProcessorExecutionOrder.After,        -1 },
        };

        private void AssertOrderingCorrect(bool startWithABeforeB, bool aAndBadjacent)
        {
            for (int iTruth = 0; iTruth < truthTable.Length; ++iTruth)
            {
                ProcessorExecutionOrder orderA = (ProcessorExecutionOrder)truthTable[iTruth][0];
                ProcessorExecutionOrder orderB = (ProcessorExecutionOrder)truthTable[iTruth][1];
                int swapValue = (int)truthTable[iTruth][2];

                MockPipelineBuilder pb = new MockPipelineBuilder();

                // Tell mock not to call validate, since we only want to order
                pb.OnValidateCalled = p => false;

                OrderableMockProcessorEchoString processorA = new OrderableMockProcessorEchoString() { Name = "ProcessorA" };
                OrderableMockProcessorEchoString processorB = new OrderableMockProcessorEchoString() { Name = "ProcessorB" };
                MockProcessor1 processorC = new MockProcessor1() { Name = "ProcessorC" };

                if (orderA == ProcessorExecutionOrder.Before)
                {
                    processorA.RunBefore = processorB;
                }
                if (orderA == ProcessorExecutionOrder.After)
                {
                    processorA.RunAfter = processorB;
                }
                if (orderB == ProcessorExecutionOrder.Before)
                {
                    processorB.RunBefore = processorA;
                }
                if (orderB == ProcessorExecutionOrder.After)
                {
                    processorB.RunAfter = processorA;
                }

                Processor[] processors = (startWithABeforeB)
                                            ? (aAndBadjacent)
                                                ? new Processor[] { processorA, processorB }
                                                : new Processor[] { processorA, processorC, processorB }
                                            : (aAndBadjacent)
                                                ? new Processor[] { processorB, processorA }
                                                : new Processor[] { processorB, processorC, processorA };

                Pipeline pipeline = null;
                Exception thrownException = null;
                try
                {
                    pipeline = pb.Build(
                        processors,
                        Enumerable.Empty<ProcessorArgument>(),
                        Enumerable.Empty<ProcessorArgument>()
                    );
                }
                catch (Exception e)
                {
                    thrownException = e;
                }

                if (swapValue < 0)
                {
                    Assert.IsNotNull(thrownException, "Expected exception when orderA=" + orderA + " and orderB=" + orderB + ", aFirst=" + startWithABeforeB + ", adjacent=" + aAndBadjacent);
                }
                else
                {
                    Assert.IsNull(thrownException, "Expected no exception when orderA=" + orderA + " and orderB=" + orderB + ", aFirst=" + startWithABeforeB + ", adjacent=" + aAndBadjacent);

                    int indexA = pipeline.Processors.IndexOf(processorA);
                    int indexB = pipeline.Processors.IndexOf(processorB);
                    if (orderA == ProcessorExecutionOrder.Before || orderB == ProcessorExecutionOrder.After)
                    {
                        Assert.IsTrue(indexA < indexB, "Expected A to come before when orderA=" + orderA + " and orderB=" + orderB);
                    }

                    else if (orderB == ProcessorExecutionOrder.Before || orderA == ProcessorExecutionOrder.After)
                    {
                        Assert.IsTrue(indexB < indexA, "Expected B to come before when orderA=" + orderA + " and orderB=" + orderB + ", aFirst=" + startWithABeforeB + ", adjacent=" + aAndBadjacent);
                    }

                    else
                    {
                        if (startWithABeforeB)
                        {
                            Assert.IsTrue(indexA < indexB, "Expected A to come before when orderA=" + orderA + " and orderB=" + orderB + ", aFirst=" + startWithABeforeB + ", adjacent=" + aAndBadjacent);
                        }
                        else
                        {
                            Assert.IsTrue(indexB < indexA, "Expected A to come before when orderA=" + orderA + " and orderB=" + orderB + ", aFirst=" + startWithABeforeB + ", adjacent=" + aAndBadjacent);
                        }
                    }
                }
            }
        }
        #endregion test helpers
    }
}
