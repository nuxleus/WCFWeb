// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageOperationSelectorTests
    {
        [TestMethod]
        [Description("HttpMessageOperationSelector.SelectOperation returns custom operation name")]
        public void SelectOperation_Returns_Custom_Operation_Name()
        {
            Message message = new HttpRequestMessage().ToMessage();

            IDispatchOperationSelector selector = new MockHttpMessageOperationSelector()
            {
                OnSelectOperation = m => { return "CustomOperation"; }
            };

            string returnedOperation = selector.SelectOperation(ref message);
            Assert.AreEqual("CustomOperation", returnedOperation, "SelectOperation should have returned custom operation name");
            Assert.IsTrue(((MockHttpMessageOperationSelector)selector).WasSelectOperationCalled, "SelectOperation in derived class was not called");
        }

        [TestMethod]
        [Description("HttpMessageOperationSelector.SelectOperation throws ArgumentNullException for null message")]
        public void SelectOperation_Null_Message_Throws()
        {
            IDispatchOperationSelector selector = new MockHttpMessageOperationSelector();
            Message message = null;
            ExceptionAssert.ThrowsArgumentNull(
                "Null WCF message should throw",
                "message",
                () =>
                {
                    selector.SelectOperation(ref message);
                });
        }

        [TestMethod]
        [Description("HttpMessageOperationSelector.SelectOperation throws InvalidOperationException for non-http message")]
        public void SelectOperation_Non_Http_Message_Throws()
        {
            IDispatchOperationSelector selector = new MockHttpMessageOperationSelector();
            Message message = Message.CreateMessage(MessageVersion.None, "notUsed");
            ExceptionAssert.ThrowsInvalidOperation(
                "WCF message without inner http message should throw",
                () =>
                {
                    selector.SelectOperation(ref message);
                });
        }

        [TestMethod]
        [Description("HttpMessageOperationSelector.SelectOperation throws if null operation is returned")]
        public void SelectOperation_Null_Return_Throws()
        {
            Message message = new HttpRequestMessage().ToMessage();

            IDispatchOperationSelector selector = new MockHttpMessageOperationSelector()
            {
                OnSelectOperation = m => { return null; }
            };
            ExceptionAssert.ThrowsInvalidOperation(
                "Null operation return should throw",
                () =>
                {
                    selector.SelectOperation(ref message);
                });
        }
    }
}
