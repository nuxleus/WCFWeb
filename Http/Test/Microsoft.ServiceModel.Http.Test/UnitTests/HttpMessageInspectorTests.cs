// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageInspectorTests
    {
        #region AfterReceiveRequest Tests

        [TestMethod]
        [Description("IDispatchMessageInspector.AfterReceiveRequest receives HttpRequestMessage from WCF Message.")]
        public void AfterReceiveRequest_Receives_HttpRequestMessage()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            Message wcfMessage = httpRequestMessage.ToMessage();
            IClientChannel channel = new MockClientChannel();
            InstanceContext context = new InstanceContext(new MockService1());

            IDispatchMessageInspector inspector = new MockHttpMessageInspector()
            {
                OnAfterReceiveRequest = actualMessage =>
                {
                    Assert.AreSame(httpRequestMessage, actualMessage, "AfterReceiveRequest did not receive the HttpRequestMessage");
                    return /*state*/ null;
                }
            };

            inspector.AfterReceiveRequest(ref wcfMessage, channel, context);
            Assert.IsTrue(((MockHttpMessageInspector)inspector).WasAfterReceiveRequestCalled, "AfterReceiveRequest in derived class was not called");
        }

        [TestMethod]
        [Description("IDispatchMessageInspector.AfterReceiveRequest can return a custom state value.")]
        public void AfterReceiveRequest_Returns_Custom_State_Value()
        {
            string stringInstance = "hello";
            Message wcfMessage = new HttpRequestMessage().ToMessage();
            IClientChannel channel = new MockClientChannel();
            InstanceContext context = new InstanceContext(new MockService1());

            IDispatchMessageInspector inspector = new MockHttpMessageInspector()
            {
                OnAfterReceiveRequest = actualMessage =>
                {
                    return stringInstance;
                }
            };

            object returnedValue = inspector.AfterReceiveRequest(ref wcfMessage, channel, context);
            Assert.IsTrue(((MockHttpMessageInspector)inspector).WasAfterReceiveRequestCalled, "AfterReceiveRequest in derived class was not called");
            Assert.AreSame(stringInstance,
                           returnedValue,
                           "AfterReceiveRequest return value is not the one we returned.");
        }

        [TestMethod]
        [Description("IDispatchMessageInspector.AfterReceiveRequest throws ArgumentNullException for null WCF message argument.")]
        public void AfterReceiveRequest_Null_Message_Throws()
        {
            IDispatchMessageInspector inspector = new MockHttpMessageInspector();
            Message wcfMessage = null;
            IClientChannel channel = new MockClientChannel();
            InstanceContext context = new InstanceContext(new MockService1());
            ExceptionAssert.ThrowsArgumentNull(
                "Null WCF message argument should throw",
                "request",
                () =>
                {
                    inspector.AfterReceiveRequest(ref wcfMessage, channel, context);
                });
        }

        [TestMethod]
        [Description("IDispatchMessageInspector.AfterReceiveRequest throws InvalidOperationException for null HttpRequestMessage argument.")]
        public void AfterReceiveRequest_Null_HttpRequestMessage_Throws()
        {
            IDispatchMessageInspector inspector = new MockHttpMessageInspector();
            Message wcfMessage = Message.CreateMessage(MessageVersion.None, "unused"); ;
            IClientChannel channel = new MockClientChannel();
            InstanceContext context = new InstanceContext(new MockService1());
            ExceptionAssert.ThrowsInvalidOperation(
                "Message without HttpRequestMessage should throw",
                () =>
                {
                    inspector.AfterReceiveRequest(ref wcfMessage, channel, context);
                });
        }

        #endregion AfterReceiveRequest Tests

        #region BeforeSendReply Tests

        [TestMethod]
        [Description("IDispatchMessageInspector.BeforeSendReply receives HttpResponseMessage from WCF Message.")]
        public void BeforeSendReply_Receives_HttpResponseMessage()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            Message wcfMessage = httpResponseMessage.ToMessage();

            IDispatchMessageInspector inspector = new MockHttpMessageInspector()
            {
                OnBeforeSendReply = (actualMessage, state) =>
                {
                    Assert.AreSame(httpResponseMessage, actualMessage, "BeforeSendReply did not receive the message we provided.");
                }
            };

            inspector.BeforeSendReply(ref wcfMessage, correlationState: null);
            Assert.AreSame(httpResponseMessage, wcfMessage.ToHttpResponseMessage(), "Expected embedded HttpResponseMessage to remain unmodified");
            Assert.IsTrue(((MockHttpMessageInspector)inspector).WasBeforeSendReplyCalled, "BeforeSentReply in derived class was not called");
        }

        [TestMethod]
        [Description("IDispatchMessageInspector.BeforeSendReply receives custom correlation state")]
        public void BeforeSendReply_Receives_Custom_CorrelationState()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            Message wcfMessage = httpResponseMessage.ToMessage();
            object correlationState = "Hello";

            IDispatchMessageInspector inspector = new MockHttpMessageInspector()
            {
                OnBeforeSendReply = (actualMessage, state) =>
                {
                    Assert.AreSame(correlationState, state, "BeforeSendReply did not receive the state we provided.");
                }
            };

            inspector.BeforeSendReply(ref wcfMessage, correlationState);
            Assert.IsTrue(((MockHttpMessageInspector)inspector).WasBeforeSendReplyCalled, "BeforeSentReply in derived class was not called");
        }

        [TestMethod]
        [Description("IDispatchMessageInspector.BeforeSendReplythrows ArgumentNullException for null WCF message argument")]
        public void BeforeSendReply_Null_Message_Throws()
        {
            IDispatchMessageInspector inspector = new MockHttpMessageInspector();
            Message wcfMessage = null;
            ExceptionAssert.ThrowsArgumentNull(
                "Null message argument should throw",
                "reply",
                () =>
                {
                    inspector.BeforeSendReply(ref wcfMessage, correlationState: null);
                });
        }

        [TestMethod]
        [Description("IDispatchMessageInspector.BeforeSendReplythrows InvalidOperationException for null HttpResponseMessage argument")]
        public void BeforeSendReply_Null_HttpResponseMessage_Throws()
        {
            IDispatchMessageInspector inspector = new MockHttpMessageInspector();
            Message wcfMessage = Message.CreateMessage(MessageVersion.None, "unused");
            ExceptionAssert.ThrowsInvalidOperation(
                "WCF message without inner Http message should throw",
                () =>
                {
                    inspector.BeforeSendReply(ref wcfMessage, correlationState: null);
                });
        }

        #endregion BeforeSendReply Tests
    }
}
