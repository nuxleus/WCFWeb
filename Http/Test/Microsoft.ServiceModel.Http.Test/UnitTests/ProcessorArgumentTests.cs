// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProcessorArgumentTests
    {
        #region Type Tests

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [Description("ProcessorArgument constructor throws if the 'name' parameter is null.")]
        public void ProcessorArgument_Name_Cannot_Be_Null()
        {
            ExceptionAssert.ThrowsArgumentNull(
                "ProcessorArgument 'name' parameter cannot be null.",
                "name",
                () =>
                {
                    ProcessorArgument arg = new ProcessorArgument(null, typeof(string));
                });
        }

        [TestMethod]
        [Description("ProcessorArgument constructor throws if the 'type' parameter is null.")]
        public void ProcessorArgument_Type_Cannot_Be_Null()
        {
            ExceptionAssert.ThrowsArgumentNull(
                "ProcessorArgument 'type' parameter cannot be null.",
                "type",
                () =>
                {
                    ProcessorArgument arg = new ProcessorArgument("AName", null);
                });
        }

        [TestMethod]
        [Description("ProcessorArgument constructor throws if the 'properties' parameter is null.")]
        public void ProcessorArgument_Properties_Cannot_Be_Null()
        {
            ExceptionAssert.ThrowsArgumentNull(
                "ProcessorArgument 'properties' parameter cannot be null.",
                "properties",
                () =>
                {
                    ProcessorArgument arg = new ProcessorArgument("AName", typeof(string), null);
                });
        }

        [TestMethod]
        [Description("ProcessorArgument constructor throws if the 'name' parameter is empty string.")]
        public void ProcessorArgument_Name_Cannot_Be_Empty_String()
        {
            ExceptionAssert.ThrowsInvalidOperation(
                "ProcessorArgument 'name' parameter cannot be an empty string.",
                () =>
                {
                    ProcessorArgument arg = new ProcessorArgument("", typeof(string));
                });
        }

        [TestMethod]
        [Description("ProcessorArgument constructor throws if the 'name' parameter is all whitespace.")]
        public void ProcessorArgument_Name_Cannot_Be_All_Whitespace()
        {
            ExceptionAssert.ThrowsInvalidOperation(
                "ProcessorArgument 'name' parameter cannot be only whitespace.",
                () =>
                {
                    ProcessorArgument arg = new ProcessorArgument(" ", typeof(string));
                });
        }

        [TestMethod]
        [Description("ProcessorArgument constructor throws if the 'type' parameter a nullable type.")]
        public void ProcessorArgument_Type_Cannot_Be_Nullable()
        {
            ExceptionAssert.ThrowsInvalidOperation(
                "ProcessorArgument 'type' parameter cannot be a nullable type.",
                () =>
                {
                    ProcessorArgument arg = new ProcessorArgument("AName", typeof(Nullable<int>));
                });
        }

        [TestMethod]
        [Description("ProcessorArgument constructor accepts generic 'type' parameters other than nullable.")]
        public void ProcessorArgument_Type_Can_Be_Generic()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(List<int>));
            Assert.AreEqual(typeof(List<int>), arg.ArgumentType, "ProcessorArgument.Type should be a generic list of integers.");
        }

        #endregion Constructor Tests

        #region Property Tests

        [TestMethod]
        [Description("ProcessorArgument.Properties should never be null.")]
        public void Properties_Should_Never_Be_Null()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(int));
            Assert.IsNotNull(arg.Properties, "ProcessorArgument.Properties should never be null.");
        }

        [TestMethod]
        [Description("ProcessorArgument.Index should be null after construction.")]
        public void Index_Should_Be_Null_After_Construction()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(int));
            Assert.IsNull(arg.Index, "ProcessorArgument.Index should be null after construction.");
        }

        [TestMethod]
        [Description("ProcessorArgument.Index should be non null after adding to a ProcessorArgumentCollection.")]
        public void Index_Should_Be_Non_Null_After_Adding_To_A_ProcessorArgumentCollection()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(int));
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetInputArguments(arg);
            ProcessorArgumentCollection collection = processor.InArguments;

            Assert.AreEqual(0, arg.Index, "ProcessorArgument.Index should be 0 after being added to the ProcessorArgumentCollection.");
        }

        [TestMethod]
        [Description("ProcessorArgument.ContainingCollection should be null after construction.")]
        public void ContainingCollection_Should_Be_Null_After_Construction()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(int));
            Assert.IsNull(arg.ContainingCollection, "ProcessorArgument.ContainingCollection should be null after construction.");
        }

        [TestMethod]
        [Description("ProcessorArgument.ContainingCollection should be non null after adding to a ProcessorArgumentCollection.")]
        public void ContainingCollection_Should_Be_Non_Null_After_Adding_To_A_ProcessorArgumentCollection()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(int));
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetInputArguments(arg);
            ProcessorArgumentCollection collection = processor.InArguments;

            Assert.AreSame(collection, arg.ContainingCollection, "ProcessorArgument.ContainingColelction should be non-null after being added to the ProcessorArgumentCollection.");
        }

        [TestMethod]
        [Description("ProcessorArgument.Name throw if set to null.")]
        public void Name_Cannot_Be_Set_To_Null()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string));

            ExceptionAssert.ThrowsArgumentNull(
                "ProcessorArgument.Name cannot be null.",
                "value",
                () =>
                {
                    arg.Name = null;
                });
        }

        [TestMethod]
        [Description("ProcessorArgument.Name throw if set to empty string.")]
        public void Name_Cannot_Be_Set_To_Empty_String()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string));

            ExceptionAssert.ThrowsInvalidOperation(
                "ProcessorArgument.Name cannot be only whitespace.",
                () =>
                {
                    arg.Name = string.Empty;
                });
        }

        [TestMethod]
        [Description("ProcessorArgument.Name throw if set to all whitespace.")]
        public void Name_Cannot_Be_Set_To_All_Whitespace()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string));
            
            ExceptionAssert.ThrowsInvalidOperation(
                "ProcessorArgument.Name cannot be only whitespace.",
                () =>
                {
                    arg.Name = " ";
                });
        }

        #endregion Property Tests

        #region Belong To ProcessorArgumentCollection Tests

        [TestMethod]
        [Description("ProcessorArgument cannot belong to multiple ProcessorArgumentCollections")]
        public void ProcessorArgument_Cannot_Belong_to_Multiple_Collections()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg);
            ProcessorArgumentCollection processor1InArguments = processor1.InArguments;
            MockNonGenericProcessor processor2 = new MockNonGenericProcessor();
            processor2.SetInputArguments(arg);

            ExceptionAssert.ThrowsInvalidOperation(
                "Adding processor argument to 2nd processor should throw",
                () =>
                {
                    ProcessorArgumentCollection processor2InArguments = processor2.InArguments;
                });
        }

        [TestMethod]
        [Description("ProcessorArgument cannot belong to both in and out ProcessorArgumentCollections")]
        public void ProcessorArgument_Cannot_Belong_to_Both_In_And_Out_Collections()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg);
            ProcessorArgumentCollection processor1InArguments = processor1.InArguments;
            processor1.SetOutputArguments(arg);

            ExceptionAssert.ThrowsInvalidOperation(
                "Adding processor argument to in and out arguments should throw",
                () =>
                {
                    ProcessorArgumentCollection processor1OutArguments = processor1.OutArguments;
                });
        }

        #endregion Belong To ProcessorArgumentCollection Tests

        #region Copy Tests

        [TestMethod]
        [Description("ProcessorArgument.Copy creates a new ProcessorArgument that is not attached to any ProcessorArgumentCollection.")]
        public void Copy_Creates_New_ProcessorArgument_Not_Attached_To_Any_Collection()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg);
            ProcessorArgumentCollection processor1InArguments = processor1.InArguments;

            ProcessorArgument argCopy = arg.Copy();

            Assert.IsNotNull(argCopy, "ProcessorArgument.Copy should not have returned null.");
            Assert.AreNotSame(arg, argCopy, "ProcessorArgument.Copy should have returned a new instance of ProcessorArgument.");
            Assert.IsNotNull(arg.ContainingCollection, "The original ProcessorArgument should have had a containing collection.");
            Assert.IsNull(argCopy.ContainingCollection, "The copied ProcessorArgument should not belong to any containing collection.");
            Assert.IsNotNull(arg.Index, "The original processor argument should have an index since it belongs to a collection.");
            Assert.IsNull(argCopy.Index, "The copied processor argument should not have an index since it does not belong to any containing collection.");
        }

        [TestMethod]
        [Description("ProcessorArgument.Copy creates a new ProcessorArgument that has the same name, type and properties as the original ProcessorArgument.")]
        public void Copy_Creates_New_ProcessorArgument_With_Same_Name_Type_And_Properties()
        {
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string), 5);
            ProcessorArgument argCopy = arg.Copy();

            Assert.IsNotNull(argCopy, "ProcessorArgument.Copy should not have returned null.");
            Assert.AreNotSame(arg, argCopy, "ProcessorArgument.Copy should have returned a new instance of ProcessorArgument.");
            Assert.AreEqual("AName", argCopy.Name, "The copied ProcessorArgument should have the same name as the original ProcessorArgument.");
            Assert.AreEqual("AName", arg.Name, "The original ProcessorArgument's name should not have changed.");
            Assert.AreEqual(typeof(string), argCopy.ArgumentType, "The copied ProcessorArgument should have the same type as the original ProcessorArgument.");
            Assert.AreEqual(typeof(string), arg.ArgumentType, "The original ProcessorArgument's type should not have changed.");
            Assert.AreEqual(1, argCopy.Properties.Count, "The copied ProcessorArgument should have the same properties as the original ProcessorArgument.");
            Assert.AreEqual(5, argCopy.Properties.Find<int>(), "The copied ProcessorArgument should have the same properties as the original ProcessorArgument.");
            Assert.AreEqual(1, arg.Properties.Count, "The original ProcessorArgument's properties should not have changed.");
            Assert.AreEqual(5, arg.Properties.Find<int>(), "The original ProcessorArgument's properties should have changed.");
        }

        [TestMethod]
        [Description("ProcessorArgument.Copy clones any properties that implement ICloneable, otherwise just passes the proprty to the new properties collection by reference.")]
        public void Copy_Honors_Cloneable_Properties()
        {
            Uri uri = new Uri("http://localhost");
            MockCloneableProperty cloneable = new MockCloneableProperty { Id = 5, Name = "SomeName" };
            ProcessorArgument arg = new ProcessorArgument("AName", typeof(string), uri, cloneable);
            ProcessorArgument argCopy = arg.Copy();

            Assert.AreEqual(2, argCopy.Properties.Count, "The copied ProcessorArgument should have the same properties as the original ProcessorArgument.");
            Assert.AreSame(uri, argCopy.Properties.Find<Uri>(), "The copied ProcessorArgument should have the same property instance as the original ProcessorArgument.");
            Assert.AreNotSame(cloneable, argCopy.Properties.Find<MockCloneableProperty>(), "The copied ProcessorArgument should have a new property instance.");
        }

        #endregion Copy Tests
    }
}
