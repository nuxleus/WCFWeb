// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProcessorTests
    {
        #region Type Tests

        #endregion Type Tests

        #region InArguments Tests

        [TestMethod]
        [Description("Processor.InArguments returns a ProcessorArgumentCollection with a Direction of Input.")]
        public void InArguments_Returns_ProcessorArgumentCollection()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetInputArguments(new ProcessorArgument("arg1", typeof(string)));

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.IsNotNull(arguments, "InArguments should never be null.");
            Assert.AreEqual(ProcessorArgumentDirection.In, arguments.Direction, "InArguments.Direction ProcessorArgumentDirection.In");
            Assert.AreEqual(1, arguments.Count, "InArguments.Count should have been 1.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns a ProcessorArgumentCollection with a Direction of Input even if the array of ProcessorArguments is empty.")]
        public void InArguments_Returns_ProcessorArgumentCollection_with_Empty_ProcessorArgument_Array()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetInputArguments(new ProcessorArgument[0]);

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.IsNotNull(arguments, "InArguments should never be null.");
            Assert.AreEqual(ProcessorArgumentDirection.In, arguments.Direction, "InArguments.Direction ProcessorArgumentDirection.In");
            Assert.AreEqual(0, arguments.Count, "InArguments.Count should have been 0.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns a ProcessorArgumentCollection with a Direction of Input even if the array of ProcessorArguments is null.")]
        public void InArguments_Returns_ProcessorArgumentCollection_with_Null_ProcessorArgument_Array()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetInputArguments(null);

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.IsNotNull(arguments, "InArguments should never be null.");
            Assert.AreEqual(ProcessorArgumentDirection.In, arguments.Direction, "InArguments.Direction ProcessorArgumentDirection.In");
            Assert.AreEqual(0, arguments.Count, "InArguments.Count should have been 0.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns the same ProcessorArgumentCollection instance every time.")]
        public void InArguments_Returns_The_Same_ProcessorArgumentCollection_Instance_Every_Time()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetInputArguments(new ProcessorArgument("arg1", typeof(string)));

            ProcessorArgumentCollection arguments1 = processor.InArguments;
            ProcessorArgumentCollection arguments2 = processor.InArguments;
            ProcessorArgumentCollection arguments3 = processor.InArguments;

            Assert.IsNotNull(arguments1, "InArguments should never be null.");
            Assert.AreSame(arguments1, arguments2, "InArguments should return the same instance every time.");
            Assert.AreSame(arguments1, arguments3, "InArguments should return the same instance every time.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns the same ProcessorArgument instances in the same order with which they were supplied.")]
        public void InArguments_Returns_Same_ProcessorArguments_In_The_Same_Order()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetInputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.IsNotNull(arguments, "InArguments should never be null.");
            Assert.AreEqual(3, arguments.Count, "InArguments.Count should have returned 3.");

            Assert.AreSame(arg1, arguments[0], "The argument should have been the same instance as was provided by OnGetInputArguments.");
            Assert.AreSame(arg2, arguments[1], "The argument should have been the same instance as was provided by OnGetInputArguments.");
            Assert.AreSame(arg3, arguments[2], "The argument should have been the same instance as was provided by OnGetInputArguments.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns ProcessorArgument instances with the Index property set.")]
        public void InArguments_Returns_ProcessorArguments_With_Index_Set()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetInputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.AreEqual(0, arguments[0].Index, "The argument index should have been determined by the order it was returned from OnGetInputArguments.");
            Assert.AreEqual(1, arguments[1].Index, "The argument index should have been determined by the order it was returned from OnGetInputArguments.");
            Assert.AreEqual(2, arguments[2].Index, "The argument index should have been determined by the order it was returned from OnGetInputArguments.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns ProcessorArgument instances with the ContainingCollection property set.")]
        public void InArguments_Returns_ProcessorArguments_With_ContainingCollection_Set()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetInputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.AreSame(arguments, arguments[0].ContainingCollection, "The argument ContainingCollection should have been set by the ProcessorArgumentCollection.");
            Assert.AreSame(arguments, arguments[1].ContainingCollection, "The argument ContainingCollection should have been set by the ProcessorArgumentCollection.");
            Assert.AreSame(arguments, arguments[2].ContainingCollection, "The argument ContainingCollection should have been set by the ProcessorArgumentCollection.");
        }

        [TestMethod]
        [Description("Processor.InArguments returns the same ProcessorArgument instances without changing the Name, Type, or Properties properties.")]
        public void InArguments_Does_Not_Change_ProcessorArguments()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetInputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.InArguments;

            Assert.IsNotNull(arguments, "InArguments should never be null.");
            Assert.AreEqual(3, arguments.Count, "InArguments.Count should have returned 3.");

            Assert.AreSame(arg1, arguments[0], "The argument should have been the same instance as was provided by OnGetInputArguments.");
            Assert.AreSame(arg2, arguments[1], "The argument should have been the same instance as was provided by OnGetInputArguments.");
            Assert.AreSame(arg3, arguments[2], "The argument should have been the same instance as was provided by OnGetInputArguments.");

            Assert.AreEqual("arg1", arguments[0].Name, "The argument name should have been the same as when it was created.");
            Assert.AreEqual("arg2", arguments[1].Name, "The argument name should have been the same as when it was created.");
            Assert.AreEqual("arg3", arguments[2].Name, "The argument name should have been the same as when it was created.");

            Assert.AreEqual(typeof(string), arguments[0].ArgumentType, "The argument type should have been the same as when it was created.");
            Assert.AreEqual(typeof(Uri), arguments[1].ArgumentType, "The argument type should have been the same as when it was created.");
            Assert.AreEqual(typeof(int), arguments[2].ArgumentType, "The argument type should have been the same as when it was created.");

            Assert.AreEqual(1, arguments[0].Properties.Count, "The argument should have the one property that it was created with.");
            Assert.AreEqual("someProperty", arguments[0].Properties.Find<string>(), "The argument should have the property value that it was created with.");

            Assert.AreEqual(2, arguments[1].Properties.Count, "The argument should have the two properties that it was created with.");
            Assert.AreEqual("someOtherProperty", arguments[1].Properties.Find<string>(), "The argument should have the property value that it was created with.");
            Assert.AreEqual(5, arguments[1].Properties.Find<int>(), "The argument should have the property value that it was created with.");

            Assert.AreEqual(0, arguments[2].Properties.Count, "The argument should not have any properties since it was not created with any.");
        }

        [TestMethod]
        [Description("Processor.InArguments throws if a null ProcessArgument is returned from OnGetInputArguments.")]
        public void InArguments_With_A_Null_ProcessorArgument_Throws()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = null;
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetInputArguments(arg1, arg2, arg3);

            ExceptionAssert.ThrowsArgumentNull(
                "InArguments should have thrown since a null ProcessorArgument was provided.",
                string.Empty,
                () =>
                {
                    ProcessorArgumentCollection arguments = processor.InArguments;
                });
        }

        [TestMethod]
        [Description("Processor.InArguments throws if there are two arguments with the same name.")]
        public void InArguments_With_Two_ProcessorArguments_With_Same_Name_Throws()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg1", typeof(int));
            processor.SetInputArguments(arg1, arg2);

            ExceptionAssert.ThrowsInvalidOperation(
                "InArguments should have thrown since there were two arguments with the same name.",
                () =>
                {
                    ProcessorArgumentCollection arguments = processor.InArguments;
                });
        }

        #endregion InArguments Tests

        #region OutArguments Tests

        [TestMethod]
        [Description("Processor.OutArguments returns a ProcessorArgumentCollection with a Direction of Output.")]
        public void OutArguments_Returns_ProcessorArgumentCollection()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetOutputArguments(new ProcessorArgument("arg1", typeof(string)));

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.IsNotNull(arguments, "OutArguments should never be null.");
            Assert.AreEqual(ProcessorArgumentDirection.Out, arguments.Direction, "OutArguments.Direction ProcessorArgumentDirection.Out");
            Assert.AreEqual(1, arguments.Count, "OutArguments.Count should have been 1.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns a ProcessorArgumentCollection with a Direction of Output even if the array of ProcessorArguments is empty.")]
        public void OutArguments_Returns_ProcessorArgumentCollection_with_Empty_ProcessorArgument_Array()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetOutputArguments(new ProcessorArgument[0]);

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.IsNotNull(arguments, "OutArguments should never be null.");
            Assert.AreEqual(ProcessorArgumentDirection.Out, arguments.Direction, "OutArguments.Direction ProcessorArgumentDirection.Out");
            Assert.AreEqual(0, arguments.Count, "OutArguments.Count should have been 0.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns a ProcessorArgumentCollection with a Direction of Output even if the array of ProcessorArguments is null.")]
        public void OutArguments_Returns_ProcessorArgumentCollection_with_Null_ProcessorArgument_Array()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetOutputArguments(null);

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.IsNotNull(arguments, "OutArguments should never be null.");
            Assert.AreEqual(ProcessorArgumentDirection.Out, arguments.Direction, "OutArguments.Direction ProcessorArgumentDirection.Out");
            Assert.AreEqual(0, arguments.Count, "OutArguments.Count should have been 0.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns the same ProcessorArgumentCollection instance every time.")]
        public void OutArguments_Returns_The_Same_ProcessorArgumentCollection_Instance_Every_Time()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.SetOutputArguments(new ProcessorArgument("arg1", typeof(string)));

            ProcessorArgumentCollection arguments1 = processor.OutArguments;
            ProcessorArgumentCollection arguments2 = processor.OutArguments;
            ProcessorArgumentCollection arguments3 = processor.OutArguments;

            Assert.IsNotNull(arguments1, "OutArguments should never be null.");
            Assert.AreSame(arguments1, arguments2, "OutArguments should return the same instance every time.");
            Assert.AreSame(arguments1, arguments3, "OutArguments should return the same instance every time.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns the same ProcessorArgument instances in the same order with which they were supplied.")]
        public void OutArguments_Returns_Same_ProcessorArguments_In_The_Same_Order()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetOutputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.IsNotNull(arguments, "OutArguments should never be null.");
            Assert.AreEqual(3, arguments.Count, "OutArguments.Count should have returned 3.");

            Assert.AreSame(arg1, arguments[0], "The argument should have been the same instance as was provided by OnGetOutputArguments.");
            Assert.AreSame(arg2, arguments[1], "The argument should have been the same instance as was provided by OnGetOutputArguments.");
            Assert.AreSame(arg3, arguments[2], "The argument should have been the same instance as was provided by OnGetOutputArguments.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns ProcessorArgument instances with the Index property set.")]
        public void OutArguments_Returns_ProcessorArguments_With_Index_Set()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetOutputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.AreEqual(0, arguments[0].Index, "The argument index should have been determined by the order it was returned from OnGetOutputArguments.");
            Assert.AreEqual(1, arguments[1].Index, "The argument index should have been determined by the order it was returned from OnGetOutputArguments.");
            Assert.AreEqual(2, arguments[2].Index, "The argument index should have been determined by the order it was returned from OnGetOutputArguments.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns ProcessorArgument instances with the ContainingCollection property set.")]
        public void OutArguments_Returns_ProcessorArguments_With_ContainingCollection_Set()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetOutputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.AreSame(arguments, arguments[0].ContainingCollection, "The argument ContainingCollection should have been set by the ProcessorArgumentCollection.");
            Assert.AreSame(arguments, arguments[1].ContainingCollection, "The argument ContainingCollection should have been set by the ProcessorArgumentCollection.");
            Assert.AreSame(arguments, arguments[2].ContainingCollection, "The argument ContainingCollection should have been set by the ProcessorArgumentCollection.");
        }

        [TestMethod]
        [Description("Processor.OutArguments returns the same ProcessorArgument instances without changing the Name, Type, or Properties properties.")]
        public void OutArguments_Does_Not_Change_ProcessorArguments()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg2", typeof(Uri), "someOtherProperty", 5);
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetOutputArguments(arg1, arg2, arg3);

            ProcessorArgumentCollection arguments = processor.OutArguments;

            Assert.IsNotNull(arguments, "OutArguments should never be null.");
            Assert.AreEqual(3, arguments.Count, "OutArguments.Count should have returned 3.");

            Assert.AreSame(arg1, arguments[0], "The argument should have been the same instance as was provided by OnGetOutputArguments.");
            Assert.AreSame(arg2, arguments[1], "The argument should have been the same instance as was provided by OnGetOutputArguments.");
            Assert.AreSame(arg3, arguments[2], "The argument should have been the same instance as was provided by OnGetOutputArguments.");

            Assert.AreEqual("arg1", arguments[0].Name, "The argument name should have been the same as when it was created.");
            Assert.AreEqual("arg2", arguments[1].Name, "The argument name should have been the same as when it was created.");
            Assert.AreEqual("arg3", arguments[2].Name, "The argument name should have been the same as when it was created.");

            Assert.AreEqual(typeof(string), arguments[0].ArgumentType, "The argument type should have been the same as when it was created.");
            Assert.AreEqual(typeof(Uri), arguments[1].ArgumentType, "The argument type should have been the same as when it was created.");
            Assert.AreEqual(typeof(int), arguments[2].ArgumentType, "The argument type should have been the same as when it was created.");

            Assert.AreEqual(1, arguments[0].Properties.Count, "The argument should have the one property that it was created with.");
            Assert.AreEqual("someProperty", arguments[0].Properties.Find<string>(), "The argument should have the property value that it was created with.");

            Assert.AreEqual(2, arguments[1].Properties.Count, "The argument should have the two properties that it was created with.");
            Assert.AreEqual("someOtherProperty", arguments[1].Properties.Find<string>(), "The argument should have the property value that it was created with.");
            Assert.AreEqual(5, arguments[1].Properties.Find<int>(), "The argument should have the property value that it was created with.");

            Assert.AreEqual(0, arguments[2].Properties.Count, "The argument should not have any properties since it was not created with any.");
        }

        [TestMethod]
        [Description("Processor.OutArguments throws if a null ProcessArgument is returned from OnGetOutputArguments.")]
        public void OutArguments_With_A_Null_ProcessorArgument_Throws()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = null;
            ProcessorArgument arg3 = new ProcessorArgument("arg3", typeof(int));
            processor.SetOutputArguments(arg1, arg2, arg3);

            ExceptionAssert.ThrowsArgumentNull(
                "OutArguments should have thrown since a null ProcessorArgument was provided.",
                string.Empty,
                () =>
                {
                    ProcessorArgumentCollection arguments = processor.OutArguments;
                });
        }

        [TestMethod]
        [Description("Processor.OutArguments throws if there are two arguments with the same name.")]
        public void OutArguments_With_Two_ProcessorArguments_With_Same_Name_Throws()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorArgument arg1 = new ProcessorArgument("arg1", typeof(string), "someProperty");
            ProcessorArgument arg2 = new ProcessorArgument("arg1", typeof(int));
            processor.SetOutputArguments(arg1, arg2);

            ExceptionAssert.ThrowsInvalidOperation(
                "OutArguments should have thrown since there were two arguments with the same name.",
                () =>
                {
                    ProcessorArgumentCollection arguments = processor.OutArguments;
                });
        }

        #endregion OutArguments Tests

        #region Initialize Tests

        [TestMethod]
        [Description("Processor fires the Initializing event when Initialize is called.")]
        public void Processor_Fires_Initializing_Event_On_Initialize()
        {
            Processor mock = new MockProcessor1();

            bool initializingCalled = false;
            object initializingObject = null;
            EventArgs initializingEventArgs = null;

            mock.Initializing = new EventHandler( 
                (obj, eArgs) => 
                {
                    initializingCalled = true;
                    initializingObject = obj;
                    initializingEventArgs = eArgs;
                });

            mock.Initialize();

            Assert.IsTrue(initializingCalled, "The Initializing event should have fired.");
            Assert.AreSame(mock, initializingObject, "The event sender should have been the processor itself.");
            Assert.AreSame(EventArgs.Empty, initializingEventArgs, "The event args should have been the EventArgs.Empty instance.");
        }

        [TestMethod]
        [Description("Processor fires the Initialized event when Initialize is called.")]
        public void Processor_Fires_Initialized_Events_On_Initialize()
        {
            Processor mock = new MockProcessor1();

            bool initializedCalled = false;
            object initializedObject = null;
            EventArgs initializedEventArgs = null;

            mock.Initialized = new EventHandler(
                (obj, eArgs) =>
                {
                    initializedCalled = true;
                    initializedObject = obj;
                    initializedEventArgs = eArgs;
                });

            mock.Initialize();

            Assert.IsTrue(initializedCalled, "The Initialized event should have fired.");
            Assert.AreSame(mock, initializedObject, "The event sender should have been the processor itself.");
            Assert.AreSame(EventArgs.Empty, initializedEventArgs, "The event args should have been the EventArgs.Empty instance.");
        }

        [TestMethod]
        [Description("Processor fires the Initializes events and sets IsInitialized in the correct order.")]
        public void Processor_Initializes_In_Correct_Order()
        {
            MockNonGenericProcessor mock = new MockNonGenericProcessor();
            int count = 0;
            
            mock.Initializing = new EventHandler(
                (obj, eArgs) =>
                {
                    Assert.AreEqual(0, count, "Count should have been 0 because the Initializing event should have been called first.");
                    Assert.IsFalse(mock.IsInitialized, "IsInitialized should be false.");
                    count++;
                });

            mock.OnInitializeAction = () =>
                {
                    Assert.AreEqual(1, count, "Count should have been 1 because the OnInitilize override should have been called second.");
                    Assert.IsFalse(mock.IsInitialized, "IsInitialized should be false.");
                    count++;
                };

            mock.Initialized = new EventHandler(
                (obj, eArgs) =>
                {
                    Assert.AreEqual(2, count, "Count should have been 2 because the Initialized event should have been called third.");
                    Assert.IsTrue(mock.IsInitialized, "IsInitialized should be true.");
                    count++;
                });

            Assert.IsFalse(mock.IsInitialized, "IsInitialized should be false.");
            mock.Initialize();
            Assert.IsTrue(mock.IsInitialized, "IsInitialized should be true.");

            Assert.AreEqual(3, count, "Count should have been 3 because the Initialized event should have been called.");
        }

        #endregion Initialize Tests

        #region Execute Tests

        [TestMethod]
        [Description("Processor.Execute throws if the input parameter is null.")]
        public void Execute_Throws_If_Input_Is_Null()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            processor.Initialize();

            ExceptionAssert.ThrowsArgumentNull(
                "Execute should have thrown since the input parameter is null.",
                "input",
                () =>
                {
                    ProcessorResult resultFromExecute = processor.Execute(null);
                });
        }

        [TestMethod]
        [Description("Processor.Execute throws if Initialize has not been called.")]
        public void Execute_Throws_If_Initialize_Has_Not_Been_Called()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();

            ExceptionAssert.ThrowsInvalidOperation(
                "Execute should have thrown since the input parameter is null.",
                () =>
                {
                    ProcessorResult resultFromExecute = processor.Execute(new object[0]);
                });
        }

        [TestMethod]
        [Description("Processor.Execute does not change the ProcessResult returned from OnExecute as long as it is not null and ProcessorStatus.Ok.")]
        public void Execute_Does_Not_Change_ProcessResult_If_Not_Null()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            Exception exception = new Exception();
            ProcessorResult resultFromOnExecute = null; 
            processor.OnExecuteAction = input =>
                {
                    resultFromOnExecute = new ProcessorResult() { Status = ProcessorStatus.Ok, Output = input, Error = exception };
                    return resultFromOnExecute;
                };

            processor.Initialize();

            object[] inputObjArray = new object[1] { "hello" };
            ProcessorResult result = processor.Execute(inputObjArray);

            Assert.IsNotNull(result, "Processor.Execute should never return null.");
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Processor.Execute should have returned the same value as returned from OnExecute.");
            Assert.AreSame(resultFromOnExecute, result, "Processor.Execute should have returned the same instance returned from OnExecute.");
            Assert.AreSame(inputObjArray, result.Output, "Processor.Execute should have returned the same instance returned from OnExecute.");
            Assert.AreSame(exception, result.Error, "Processor.Execute should have returned the same instance returned from OnExecute.");
        }

        [TestMethod]
        [Description("Processor.Execute allows the Output and Error properties on ProcessResult to be null.")]
        public void Execute_Allows_Null_Output_And_Null_Error_On_ProcessResult()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            Exception exception = new Exception();
            ProcessorResult resultFromOnExecute = null;
            processor.OnExecuteAction = input =>
            {
                resultFromOnExecute = new ProcessorResult() { Status = ProcessorStatus.Ok, Output = null, Error = null };
                return resultFromOnExecute;
            };

            processor.Initialize();

            object[] inputObjArray = new object[1] { "hello" };
            ProcessorResult result = processor.Execute(inputObjArray);

            Assert.IsNotNull(result, "Processor.Execute should never return null.");
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Processor.Execute should have returned the same value as returned from OnExecute.");
            Assert.AreSame(resultFromOnExecute, result, "Processor.Execute should have returned the same instance returned from OnExecute.");
            Assert.IsNull(result.Output, "Processor.Execute should have returned the same instance returned from OnExecute.");
            Assert.IsNull(result.Error, "Processor.Execute should have returned the same instance returned from OnExecute.");
        }

        [TestMethod]
        [Description("Processor.Execute allows the Output and Error properties on ProcessResult to be null.")]
        public void Execute_Default_ProcessResult_Is_Status_Ok_With_Null_Output()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            Exception exception = new Exception();
            processor.OnExecuteAction = input => new ProcessorResult();

            processor.Initialize();
            ProcessorResult result = processor.Execute(new object[0]);

            Assert.IsNotNull(result, "Processor.Execute should never return null.");
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Processor.Execute should have returned the ProcessorStatus.Ok.");
            Assert.IsNull(result.Output, "Processor.Execute should have returned a null Output.");
            Assert.IsNull(result.Error, "Processor.Execute should have returned a null exception.");
        }

        #endregion Execute Tests

        #region OnError Tests

        [TestMethod]
        [Description("Processor calls OnError if the Execute method returned a ProcessorResult with a ProcessorStatus of ProcessorStatus.Error.")]
        public void OnError_Called_If_Execute_Returns_ProcessorStatus_Error()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorResult resultFromOnError = null;
            processor.OnErrorAction = result => resultFromOnError = result;
            processor.OnExecuteAction = input => new ProcessorResult { Status = ProcessorStatus.Error };

            processor.Initialize();
            ProcessorResult resultFromExecute = processor.Execute(new object[0]);

            Assert.IsNotNull(resultFromExecute, "OnError should have been called.");
            Assert.AreEqual(ProcessorStatus.Error, resultFromOnError.Status, "The Status should have been ProcessorStatus.Error");
            Assert.IsNull(resultFromOnError.Error, "The Exception should not have been set of the ProcessorResult.");
            Assert.IsNull(resultFromOnError.Output, "The Output should not have been set of the ProcessorResult.");
            Assert.AreSame(resultFromOnError, resultFromExecute, "The ProcessorResult from Execute should be the same instance as the one from OnError");
        }

        [TestMethod]
        [Description("Processor calls OnError if the Execute method throws.")]
        public void OnError_Called_If_Execute_Throws()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorResult resultFromOnError = null;
            InvalidOperationException exception = new InvalidOperationException();
            processor.OnErrorAction = result => resultFromOnError = result;
            processor.OnExecuteAction = input => { throw exception; };

            processor.Initialize();
            ProcessorResult resultFromExecute = processor.Execute(new object[0]);

            Assert.IsNotNull(resultFromExecute, "OnError should have been called.");
            Assert.AreEqual(ProcessorStatus.Error, resultFromOnError.Status, "The Status should have been ProcessorStatus.Error");
            Assert.IsNotNull(resultFromOnError.Error, "The Exception should have been set of the ProcessorResult.");
            Assert.AreSame(exception, resultFromOnError.Error, "The Exception should have been the same instance as the one thrown from within OnExecute.");
            Assert.IsNull(resultFromOnError.Output, "The Output should not have been set of the ProcessorResult.");
            Assert.AreSame(resultFromOnError, resultFromExecute, "The ProcessorResult from Execute should be the same instance as the one from OnError");
        }

        [TestMethod]
        [Description("Processor calls OnError if the Execute method returns a null ProcessorResult.")]
        public void OnError_Called_If_Execute_Returns_Null()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorResult resultFromOnError = null;
            processor.OnErrorAction = result => resultFromOnError = result;
            processor.OnExecuteAction = input => null;

            processor.Initialize();
            ProcessorResult resultFromExecute = processor.Execute(new object[0]);

            Assert.IsNotNull(resultFromExecute, "OnError should have been called.");
            Assert.AreEqual(ProcessorStatus.Error, resultFromOnError.Status, "The Status should have been ProcessorStatus.Error");
            Assert.IsNotNull(resultFromOnError.Error, "The Exception should have been set of the ProcessorResult.");
            Assert.IsInstanceOfType(resultFromOnError.Error, typeof(InvalidOperationException), "The Exception should have been an InvalidOperationException.");
            Assert.IsNull(resultFromOnError.Output, "The Output should not have been set of the ProcessorResult.");
            Assert.AreSame(resultFromOnError, resultFromExecute, "The ProcessorResult from Execute should be the same instance as the one from OnError");
        }

        [TestMethod]
        [Description("Processor does not call OnError if the Execute method returned a ProcessorResult with a ProcessorStatus of ProcessorStatus.Ok.")]
        public void OnError_Not_Called_If_Execute_Returns_ProcessorStatus_Ok()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();
            ProcessorResult resultFromOnError = null;
            processor.OnErrorAction = result => resultFromOnError = result;
            processor.OnExecuteAction = input => new ProcessorResult { Status = ProcessorStatus.Ok };

            processor.Initialize();
            ProcessorResult resultFromExecute = processor.Execute(new object[0]);

            Assert.IsNull(resultFromOnError, "OnError should not have run.");
            Assert.IsNotNull(resultFromExecute, "There should have been a non-null result.");
        }

        [TestMethod]
        [Description("Processor does not call OnError if the Execute method returned a ProcessorResult with a ProcessorStatus of ProcessorStatus.Ok.")]
        public void OnError_ProcessorResult_Is_Not_Changed_By_Execute()
        {
            MockNonGenericProcessor processor = new MockNonGenericProcessor();

            Exception executionException = new Exception();
            object[] executionOutput = new object[0];
            processor.OnExecuteAction = input => new ProcessorResult
            {
                Status = ProcessorStatus.Error,
                Output = executionOutput,
                Error = executionException
            };

            Exception onErrorException = new Exception();
            object[] onErrorOutput = new object[0];
            processor.OnErrorAction = result =>
            {
                Assert.AreEqual(ProcessorStatus.Error, result.Status, "The result status should have been ProcessorStatus.Error.");
                Assert.AreSame(result.Error, executionException, "The Error should have been the same instance as the one set in OnExecute.");
                Assert.AreSame(result.Output, executionOutput, "The Output should have been the same instance as the one set in OnExecute.");
                result.Error = onErrorException;
                result.Output = onErrorOutput;
            };
              
            processor.Initialize();
            ProcessorResult resultFromExecute = processor.Execute(new object[0]);

            Assert.IsNotNull(resultFromExecute, "There should have been a non-null result.");
            Assert.AreEqual(ProcessorStatus.Error, resultFromExecute.Status, "The result status should have been ProcessorStatus.Error.");
            Assert.AreSame(resultFromExecute.Error, onErrorException, "The Error should have been the same instance as the one set in OnError.");
            Assert.AreSame(resultFromExecute.Output, onErrorOutput, "The Output should have been the same instance as the one set in OnError.");
        }

        #endregion OnError Tests
    }
}
