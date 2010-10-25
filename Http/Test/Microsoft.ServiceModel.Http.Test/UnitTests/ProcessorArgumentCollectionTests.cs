// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProcessorArgumentCollectionTests
    {
        #region Type Tests

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [Description("ProcessorArgumentCollection cannot be created with two arguments with the same name. ")]
        public void ProcessorArgumentCollection_Cannot_Be_Created_With_Two_Arguments_With_The_Same_Name()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            ProcessorArgument arg2 = new ProcessorArgument("AName", typeof(int));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg2);

            ExceptionAssert.ThrowsInvalidOperation(
                "Creating the argument collection with two arguments with the same name should throw",
                () =>
                {
                    ProcessorArgumentCollection processor1InArguments = processor1.InArguments;
                });
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection cannot be created with the same argument used twice. ")]
        public void ProcessorArgumentCollection_Cannot_Be_Created_With_The_Same_Argument_Twice()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg1);

            ExceptionAssert.ThrowsInvalidOperation(
                "Creating the argument collection with the same argument used twice should throw",
                () =>
                {
                    ProcessorArgumentCollection processor1InArguments = processor1.InArguments;
                });
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection cannot have two arguments with the same name. ")]
        public void ProcessorArgumentCollection_Cannot_Have_Two_Arguments_With_The_Same_Name()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            ProcessorArgument arg2 = new ProcessorArgument("AnotherName", typeof(int));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg2);
            ProcessorArgumentCollection processor1InArguments = processor1.InArguments;

            ExceptionAssert.ThrowsInvalidOperation(
                "Setting the argument name to something already in the argument collection should throw",
                () =>
                {
                    arg2.Name = "AName";
                });
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection can have two arguments with the same name but different case. ")]
        public void ProcessorArgumentCollection_Can_Have_Two_Arguments_With_The_Same_Name_Different_Case()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            ProcessorArgument arg2 = new ProcessorArgument("AnotherName", typeof(int));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg2);
            ProcessorArgumentCollection processor1InArguments = processor1.InArguments;
            arg2.Name = "aname";
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection can have two arguments with the same name but different case. ")]
        public void ProcessorArgumentCollection_Can_Have_No_Arguments()
        {
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(new ProcessorArgument[0]);
            ProcessorArgumentCollection processor1InArguments = processor1.InArguments;

            Assert.AreEqual(0, processor1InArguments.Count, "The count or processor arguments should be zero.");
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection throws if a null ProcessArgument is added.")]
        public void ProcessorArgumentCollection_With_A_Null_ProcessorArgument_Throws()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = null;
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetInputArguments(arg1, arg2, arg3);

            ExceptionAssert.ThrowsArgumentNull(
                "ProcessorArgumentCollection should have thrown since a null ProcessorArgument was provided.",
                string.Empty,
                () =>
                {
                    ProcessorArgumentCollection arguments = processor.InArguments;
                });
        }

        #endregion Constructor Tests

        #region Index Tests

        [TestMethod]
        [Description("ProcessorArgumentCollection.Index will return the ProcessorArgument. ")]
        public void Index_Returns_Argument()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            ProcessorArgument arg2 = new ProcessorArgument("AnotherName", typeof(int));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg2);
            ProcessorArgument processor1Arg = processor1.InArguments["AName"];

            Assert.AreSame(arg1, processor1Arg, "ProcessorArgumentCollection.Index should have returned the same instance.");
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection.Index is case sensitive.")]
        public void Index_Is_Case_Sensitive()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            ProcessorArgument arg2 = new ProcessorArgument("AnotherName", typeof(int));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg2);
            ProcessorArgument processor1Arg = processor1.InArguments["Aname"];

            Assert.IsNull(processor1Arg, "ProcessorArgumentCollection.Index should have returned null because the names differed by case.");
        }

        [TestMethod]
        [Description("ProcessorArgumentCollection.Index returns null if there is no argument with the given name.")]
        public void Index_Returns_Null_For_Non_Existing_Arguments()
        {
            ProcessorArgument arg1 = new ProcessorArgument("AName", typeof(string));
            ProcessorArgument arg2 = new ProcessorArgument("AnotherName", typeof(int));
            MockNonGenericProcessor processor1 = new MockNonGenericProcessor();
            processor1.SetInputArguments(arg1, arg2);
            ProcessorArgument processor1Arg = processor1.InArguments["SomeOtherName"];

            Assert.IsNull(processor1Arg, "ProcessorArgumentCollection.Index should have returned null because there is no argument with that name.");
        }

        #endregion Index Tests
    }
}
