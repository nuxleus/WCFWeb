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
    public class PipelineBindingCollectionTests
    {
        #region Type Tests

        #endregion Type Tests

        #region Bind Tests

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind can bind if the 'outArgument' ProcessorArgument type is assignable to the type of the 'inArgument' ProcessorArgument.")]
        public void Bind_With_OutArgument_Type_Assignable_To_InArgument_Type()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(Uri)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(object)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).First(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).First(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will not throw if called twice with the same arguments.")]
        public void Bind_Is_Idempotent()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).First(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).First(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).First(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).First(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind is equivalent to calling Bind, Unbind, and Bind again with the same arguments.")]
        public void Bind_Unbind_Bind_Is_Equivalent_To_Bind()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).Single(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).Single(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");

            bindings.UnbindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(0, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should not have been bound to any other arguments.");
            Assert.AreEqual(0, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should not have been bound to any other arguments.");

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).Single(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).Single(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind can be called several times with the same inArgument.")]
        public void Bind_Can_Bind_One_InArgument_Multiple_Times()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            MockNonGenericProcessor p3 = new MockNonGenericProcessor();
            p3.SetInputArguments(new ProcessorArgument("p3In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2, p3);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p3.InArguments[0]);
            bindings.BindArguments(p2.OutArguments[0], p3.InArguments[0]);
            Assert.AreEqual(2, bindings.GetBoundToArguments(p3.InArguments[0]).Count(), "Processor3 InArgument1 should have been bound to two other arguments.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p3.InArguments[0]).First(), "Processor3 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.OutArguments[0], bindings.GetBoundToArguments(p3.InArguments[0]).Skip(1).First(), "Processor3 InArgument1 should have been bound to Processor2 OutArgument1.");
            Assert.AreSame(p3.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).Single(), "Processor1 OutArgument1 should have been bound to Processor3 InArgument1.");
            Assert.AreSame(p3.InArguments[0], bindings.GetBoundToArguments(p2.OutArguments[0]).Single(), "Processor2 OutArgument1 should have been bound to Processor3 InArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind can be called several times with the same outArgument.")]
        public void Bind_Can_Bind_One_OutArgument_Multiple_Times()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            MockNonGenericProcessor p3 = new MockNonGenericProcessor();
            p3.SetInputArguments(new ProcessorArgument("p3In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2, p3);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            bindings.BindArguments(p1.OutArguments[0], p3.InArguments[0]);
            Assert.AreEqual(2, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to two other arguments.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).First(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");
            Assert.AreSame(p3.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).Skip(1).First(), "Processor1 OutArgument1 should have been bound to Processor3 InArgument1.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).Single(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p3.InArguments[0]).Single(), "Processor3 InArgument1 should have been bound to Processor1 OutArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the out argument doesn't belong to a particular processor.")]
        public void Bind_With_Out_Argument_Not_Attached_To_Processors_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the out arugment does not belong to a processor",
                () =>
                {
                    bindings.BindArguments(new ProcessorArgument("someName", typeof(string)), p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the in argument doesn't belong to a particular processor.")]
        public void Bind_With_In_Argument_Not_Attached_To_Processors_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the in arugment does not belong to a processor",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], new ProcessorArgument("someName", typeof(string)));
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the in Processor does not belong to a particular ProcessorCollection.")]
        public void Bind_With_In_Argument_On_Processors_Not_In_A_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the in arugment processor does not belong to any processor collection",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the out Processor does not belong to a particular ProcessorCollection.")]
        public void Bind_With_Out_Argument_On_Processors_Not_In_A_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the out arugment processor does not belong to any processor collection",
                () =>
                {
                    bindings.BindArguments(p2.OutArguments[0], p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the in Processor that the arguments belong to do not belong to the same ProcessorCollection.")]
        public void Bind_With_In_Arguments_On_Processors_Not_In_The_Same_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));
            ProcessorCollection otherCollection = new ProcessorCollection(new MockNonGenericProcessor(), p2);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the in arugment processor belongs to a different processor collection",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the out Processor that the arguments belong to do not belong to the same ProcessorCollection.")]
        public void Bind_With_Out_Arguments_On_Processors_Not_In_The_Same_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2In1", typeof(string)));
            ProcessorCollection otherCollection = new ProcessorCollection(new MockNonGenericProcessor(), p2);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the out arugment processor belongs to a different processor collection",
                () =>
                {
                    bindings.BindArguments(p2.OutArguments[0], p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the arguments belong to the same processor.")]
        public void Bind_With_Arguments_On_The_Same_Processor_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the in arugment and out argument belong to the same processor",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the 'inArgument' parameter is not an ProcessorArgument from a ProcessorArgumentCollection with a direction of 'In'.")]
        public void Bind_With_InArgument_That_Is_Not_In_Direction_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the in arugment is not in the in direction.",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], p2.OutArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the 'inArgument' parameter is not an ProcessorArgument from a ProcessorArgumentCollection with a direction of 'In'.")]
        public void Bind_With_OutArgument_That_Is_Not_Out_Direction_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the out arugment is not in the out direction.",
                () =>
                {
                    bindings.BindArguments(p1.InArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the 'outArgument' ProcessorArgument type is not assignable to the type of the 'inArgument' ProcessorArgument.")]
        public void Bind_With_OutArgument_Type_Not_Assignable_To_InArgument_Type_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(object)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(Uri)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because the out arugment is not assignable to the in argument.",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Bind will throw if the 'inArgument' ProcessorArgument comes before the 'outArgument' ProcessorArgument in the ProcessorCollection.")]
        public void Bind_With_InArgument_Processor_Before_OutArgument_Processor_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p2, p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Bind should have thrown because processor 1 comes after processor 2.",
                () =>
                {
                    bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        #endregion Bind Tests

        #region Unbind Tests

        [TestMethod]
        [Description("ProcessorBindingCollection.Unbind removes the binding if the arguments are already bound.")]
        public void UnBind_Removes_The_Binding_If_Arguments_Are_Bound()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).Single(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).Single(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");

            bindings.UnbindArguments(p1.OutArguments[0], p2.InArguments[0]);
            Assert.AreEqual(0, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should not have been bound to any other arguments.");
            Assert.AreEqual(0, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should not have been bound to any other arguments.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Unbind removes only the binding on the inArgument that is associated with the outArgument.")]
        public void UnBind_Does_Not_Remove_Other_Bindings_On_An_InArgument()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            MockNonGenericProcessor p3 = new MockNonGenericProcessor();
            p3.SetInputArguments(new ProcessorArgument("p3In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2, p3);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p3.InArguments[0]);
            bindings.BindArguments(p2.OutArguments[0], p3.InArguments[0]);

            bindings.UnbindArguments(p1.OutArguments[0], p3.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.OutArguments[0]).Count(), "Processor2 OutArgument1 should not have been bound to any other arguments.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p3.InArguments[0]).Count(), "Processor3 InArgument1 should not have been bound to any other arguments.");
            Assert.AreSame(p2.OutArguments[0], bindings.GetBoundToArguments(p3.InArguments[0]).Single(), "Processor3 InArgument1 should have been bound to Processor2 OutArgument1.");
            Assert.AreSame(p3.InArguments[0], bindings.GetBoundToArguments(p2.OutArguments[0]).Single(), "Processor2 OutArgument1 should have been bound to Processor3 InArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Unbind removes only the binding on the outArgument that is associated with the inArgument.")]
        public void UnBind_Does_Not_Remove_Other_Bindings_On_An_OutArgument()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            MockNonGenericProcessor p3 = new MockNonGenericProcessor();
            p3.SetInputArguments(new ProcessorArgument("p3In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2, p3);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p1.OutArguments[0], p2.InArguments[0]);
            bindings.BindArguments(p1.OutArguments[0], p3.InArguments[0]);

            bindings.UnbindArguments(p1.OutArguments[0], p3.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should have been bound to one other argument.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.InArguments[0]).Count(), "Processor2 InArgument1 should have been bound to one other argument.");
            Assert.AreSame(p1.OutArguments[0], bindings.GetBoundToArguments(p2.InArguments[0]).Single(), "Processor2 InArgument1 should have been bound to Processor1 OutArgument1.");
            Assert.AreSame(p2.InArguments[0], bindings.GetBoundToArguments(p1.OutArguments[0]).Single(), "Processor1 OutArgument1 should have been bound to Processor2 InArgument1.");
        }

        [TestMethod]
        [Description("ProcessorBindingCollection.Unbind does nothing if the arguments aren't already bound.")]
        public void UnBind_Does_Nothing_If_Arguments_Are_Not_Bound()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            MockNonGenericProcessor p3 = new MockNonGenericProcessor();
            p3.SetInputArguments(new ProcessorArgument("p3In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2, p3);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            bindings.BindArguments(p2.OutArguments[0], p3.InArguments[0]);
            Assert.AreEqual(0, bindings.GetBoundToArguments(p1.OutArguments[0]).Count(), "Processor1 OutArgument1 should not have been bound to any other arguments.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.OutArguments[0]).Count(), "Processor2 OutArgument1 should not have been bound to any other arguments.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p3.InArguments[0]).Count(), "Processor3 InArgument1 should not have been bound to any other arguments.");
            Assert.AreSame(p2.OutArguments[0], bindings.GetBoundToArguments(p3.InArguments[0]).Single(), "Processor3 InArgument1 should have been bound to Processor2 OutArgument1.");
            Assert.AreSame(p3.InArguments[0], bindings.GetBoundToArguments(p2.OutArguments[0]).Single(), "Processor2 OutArgument1 should have been bound to Processor3 InArgument1.");

            bindings.UnbindArguments(p1.OutArguments[0], p3.InArguments[0]);
            Assert.AreEqual(1, bindings.GetBoundToArguments(p2.OutArguments[0]).Count(), "Processor2 OutArgument1 should not have been bound to any other arguments.");
            Assert.AreEqual(1, bindings.GetBoundToArguments(p3.InArguments[0]).Count(), "Processor3 InArgument1 should not have been bound to any other arguments.");
            Assert.AreSame(p2.OutArguments[0], bindings.GetBoundToArguments(p3.InArguments[0]).Single(), "Processor3 InArgument1 should have been bound to Processor2 OutArgument1.");
            Assert.AreSame(p3.InArguments[0], bindings.GetBoundToArguments(p2.OutArguments[0]).Single(), "Processor2 OutArgument1 should have been bound to Processor3 InArgument1.");

        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the out argument doesn't belong to a particular processor.")]
        public void Unbind_With_Out_Argument_Not_Attached_To_Processors_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the out arugment does not belong to a processor",
                () =>
                {
                    bindings.UnbindArguments(new ProcessorArgument("someName", typeof(string)), p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the in argument doesn't belong to a particular processor.")]
        public void Unbind_With_In_Argument_Not_Attached_To_Processors_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the in arugment does not belong to a processor",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], new ProcessorArgument("someName", typeof(string)));
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the in Processor does not belong to a particular ProcessorCollection.")]
        public void Unbind_With_In_Argument_On_Processors_Not_In_A_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the in arugment processor does not belong to any processor collection",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the out Processor does not belong to a particular ProcessorCollection.")]
        public void Unbind_With_Out_Argument_On_Processors_Not_In_A_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the out arugment processor does not belong to any processor collection",
                () =>
                {
                    bindings.UnbindArguments(p2.OutArguments[0], p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the in Processor that the arguments belong to do not belong to the same ProcessorCollection.")]
        public void Unbind_With_In_Arguments_On_Processors_Not_In_The_Same_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));
            ProcessorCollection otherCollection = new ProcessorCollection(new MockNonGenericProcessor(), p2);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the in arugment processor belongs to a different processor collection",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the out Processor that the arguments belong to do not belong to the same ProcessorCollection.")]
        public void Unbind_With_Out_Arguments_On_Processors_Not_In_The_Same_ProcessorCollection_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2In1", typeof(string)));
            ProcessorCollection otherCollection = new ProcessorCollection(new MockNonGenericProcessor(), p2);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the out arugment processor belongs to a different processor collection",
                () =>
                {
                    bindings.UnbindArguments(p2.OutArguments[0], p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the arguments belong to the same processor.")]
        public void Unbind_With_Arguments_On_The_Same_Processor_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));
            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the in arugment and out argument belong to the same processor",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], p1.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the 'inArgument' parameter is not an ProcessorArgument from a ProcessorArgumentCollection with a direction of 'In'.")]
        public void Unbind_With_InArgument_That_Is_Not_In_Direction_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetOutputArguments(new ProcessorArgument("p2Out1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the in arugment is not in the in direction.",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], p2.OutArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the 'inArgument' parameter is not an ProcessorArgument from a ProcessorArgumentCollection with a direction of 'In'.")]
        public void Unbind_With_OutArgument_That_Is_Not_Out_Direction_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetInputArguments(new ProcessorArgument("p1In1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the out arugment is not in the out direction.",
                () =>
                {
                    bindings.UnbindArguments(p1.InArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the 'outArgument' ProcessorArgument type is not assignable to the type of the 'inArgument' ProcessorArgument.")]
        public void Unbind_With_OutArgument_Type_Not_Assignable_To_InArgument_Type_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(object)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(Uri)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1, p2);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because the out arugment is not assignable to the in argument.",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        [TestMethod]
        [Description("PipelineBindingCollection.Unbind will throw if the 'inArgument' ProcessorArgument comes before the 'outArgument' ProcessorArgument in the ProcessorCollection.")]
        public void Unbind_With_InArgument_Processor_Before_OutArgument_Processor_Throws()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            MockNonGenericProcessor p2 = new MockNonGenericProcessor();
            p2.SetInputArguments(new ProcessorArgument("p2In1", typeof(string)));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p2, p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            ExceptionAssert.ThrowsInvalidOperation(
                "Unbind should have thrown because processor 1 comes after processor 2.",
                () =>
                {
                    bindings.UnbindArguments(p1.OutArguments[0], p2.InArguments[0]);
                });
        }

        #endregion Unbind Tests

        #region GetBoundToArguments Tests

        [TestMethod]
        [Description("ProcessorBindingCollection.GetBoundToArguments never returns null.")]
        public void GetBoundToArguments_Never_Returns_Null()
        {
            MockNonGenericProcessor p1 = new MockNonGenericProcessor();
            p1.SetOutputArguments(new ProcessorArgument("p1Out1", typeof(string)));

            ProcessorArgument arg2 = new ProcessorArgument("arg1", typeof(string));

            ProcessorCollection collection = new ProcessorCollection(new MockNonGenericProcessor(), p1);
            PipelineBindingCollection bindings = new PipelineBindingCollection(collection);

            IEnumerable<ProcessorArgument> boundArguments = bindings.GetBoundToArguments(p1.OutArguments[0]);
            Assert.IsNotNull(boundArguments, "ProcessorBindingCollection.GetBoundArguments should never return null.");
            Assert.AreEqual(0, boundArguments.Count(), "ProcessorBindingCollection.GetBoundArguments should have returned an empty collection.");

            boundArguments = bindings.GetBoundToArguments(arg2);
            Assert.IsNotNull(boundArguments, "ProcessorBindingCollection.GetBoundArguments should never return null.");
            Assert.AreEqual(0, boundArguments.Count(), "ProcessorBindingCollection.GetBoundArguments should have returned an empty collection.");
        }

        #endregion GetBoundToArguments Tests
    }
}
