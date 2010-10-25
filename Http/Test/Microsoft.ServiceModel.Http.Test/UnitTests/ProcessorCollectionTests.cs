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
    public class ProcessorCollectionTests
    {
        #region Type Tests

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [Description("ProcessorCollection cannot be created the containing processor in the processor collection.")]
        public void ProcessorCollection_Cannot_Be_Created_With_The_ContainingProcessor_In_The_Processor_Collection()
        {
            Processor processor1 = new MockNonGenericProcessor();
            Processor processor2 = new MockNonGenericProcessor();
            Processor containingProcessor = new MockNonGenericProcessor();

            ExceptionAssert.ThrowsInvalidOperation(
                "Creating the ProcessorCollection with the containing processor in the list of processors should throw.",
                () =>
                {
                    ProcessorCollection processorCollection = new ProcessorCollection(containingProcessor, processor1, processor2, containingProcessor);
                });
        }

        [TestMethod]
        [Description("ProcessorCollection cannot be created with the same processor added more than once to the collection.")]
        public void ProcessorCollection_Cannot_Be_Created_With_A_Processor_Added_Twice()
        {
            Processor processor1 = new MockNonGenericProcessor();
            Processor processor2 = new MockNonGenericProcessor();
            Processor containingProcessor = new MockNonGenericProcessor();

            ExceptionAssert.ThrowsInvalidOperation(
                "Creating the ProcessorCollection with the processor2 in the list twice should throw.",
                () =>
                {
                    ProcessorCollection processorCollection = new ProcessorCollection(containingProcessor, processor1, processor2, processor1);
                });
        }

        [TestMethod]
        [Description("ProcessorCollection can be created with zero processors in the collection.")]
        public void ProcessorCollection_Can_Have_No_Processors()
        {
            Processor containingProcessor = new MockNonGenericProcessor();
            ProcessorCollection processorCollection = new ProcessorCollection(containingProcessor);

            Assert.AreEqual(0, processorCollection.Count, "ProcessorCollection.Count should have been zero.");
        }


        [TestMethod]
        [Description("ProcessorCollection cannot be created with a null container processor.")]
        public void ProcessorCollection_With_Null_ContainingProcessor_Throws()
        {
            Processor processor1 = new MockNonGenericProcessor();
            Processor processor2 = new MockNonGenericProcessor();

            ExceptionAssert.ThrowsArgumentNull(
                "Creating the ProcessorCollection with a null container processor should throw.",
                "containerProcessor",
                () =>
                {
                    ProcessorCollection processorCollection = new ProcessorCollection(null, processor1, processor2, processor1);
                });
        }

        [TestMethod]
        [Description("ProcessorCollection cannot be created with a list of processors.")]
        public void ProcessorCollection_With_Null_Processors_Throws()
        {
            Processor processor1 = new MockNonGenericProcessor();
            Processor processor2 = new MockNonGenericProcessor();
            Processor containingProcessor = new MockNonGenericProcessor();

            ExceptionAssert.ThrowsArgumentNull(
                "Creating the ProcessorCollection with a null list of processors should throw.",
                "list",
                () =>
                {
                    ProcessorCollection processorCollection = new ProcessorCollection(containingProcessor, null);
                });
        }

        [TestMethod]
        [Description("ProcessorCollection cannot be created with a null processor added to the collection.")]
        public void ProcessorCollection_Cannot_Be_Created_With_A_Null_Processor()
        {
            Processor processor1 = new MockNonGenericProcessor();
            Processor processor2 = new MockNonGenericProcessor();
            Processor containingProcessor = new MockNonGenericProcessor();

            ExceptionAssert.ThrowsArgumentNull(
                "Creating the ProcessorCollection with a null processor in the list should throw.",
                string.Empty,
                () =>
                {
                    ProcessorCollection processorCollection = new ProcessorCollection(containingProcessor, processor1, null, processor2);
                });
        }

        #endregion Constructor Tests
    }
}
