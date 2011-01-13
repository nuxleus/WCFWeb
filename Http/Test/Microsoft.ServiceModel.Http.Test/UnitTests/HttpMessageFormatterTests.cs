// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
  
    [TestClass]
    public class HttpMessageFormatterTests
    {
        #region DeserializeRequest Tests

        [TestMethod]
        [Description("IDispatchMessageFormatter.DeserializeRequest receives HttpRequestMessage and message parameters")]
        public void DeserializeRequest_Receives_Message_And_Parameters()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            Message wcfMessage = httpRequestMessage.ToMessage();
            object[] messageParameters = new object[] { "hello", 5.0 };

            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter()
            {
                OnDeserializeRequest = (msg, parameters) =>
                {
                    Assert.AreSame(httpRequestMessage, msg, "DeserializeRequest did not receive the HttpRequestMessage we specified");
                    Assert.AreSame(messageParameters, parameters, "DeserializeRequest did not receive the parameters we specified");
                }
            };

            formatter.DeserializeRequest(wcfMessage, messageParameters);
            Assert.IsTrue(((MockHttpMessageFormatter)formatter).WasDeserializeRequestCalled, "DeserializeRequest in derived class was not called");
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.DeserializeRequest throws ArgumentNullException for null WCF message")]
        public void DeserializeRequest_Null_Message_Throws()
        {
            object[] parameters = new object[] { "hello", 5.0 };
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            ExceptionAssert.ThrowsArgumentNull(
                "Null message argument should throw",
                "message",
                () =>
                {
                    formatter.DeserializeRequest(/*message*/ null, parameters);
                });
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.DeserializeRequest throws ArgumentNullException for null parameters")]
        public void DeserializeRequest_Null_Parameters_Throws()
        {
            Message wcfMessage = new HttpRequestMessage().ToMessage();
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            ExceptionAssert.ThrowsArgumentNull(
                 "Null parameters argument should throw",
                 "parameters",
                 () =>
                 {
                     formatter.DeserializeRequest(wcfMessage, parameters: null);
                 });
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.DeserializeRequest throws InvalidOperationException for null HttpRequestMessage")]
        public void DeserializeRequest_Null_HttpRequestMessage_Throws()
        {
            Message wcfMessage = Message.CreateMessage(MessageVersion.None, "unused");
            object[] parameters = new object[] { "hello", 5.0 };
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            ExceptionAssert.ThrowsInvalidOperation(
                "Non-http message should throw",
                () =>
                {
                    formatter.DeserializeRequest(wcfMessage, parameters);
                });
        }

        #endregion DeserializeRequest Tests

        #region SerializeReply Tests

        [TestMethod]
        [Description("IDispatchMessageFormatter.SerializeReply receives parameters and result")]
        public void SerializeReply_Receives_Parameters_And_Result()
        {
            object[] messageParameters = new object[] { "hello", 5.0 };
            string messageResult = "hello";
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter()
            {
                OnSerializeReply = (parameters, result, response) =>
                {
                    Assert.AreSame(messageParameters, parameters, "SerializeReply did not receive the input parameters");
                    Assert.AreSame(messageResult, result, "SerializeReply did not receive the input result");
                    Assert.IsNotNull(response, "Response should not be null");
                }
            };
            Message responseMessage = formatter.SerializeReply(MessageVersion.None, messageParameters, messageResult);
            Assert.IsTrue(((MockHttpMessageFormatter) formatter).WasSerializeReplyCalled, "SerializeReply on derived class was not called");
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.SerializeReply returns valid HttpResponseMessage")]
        public void SerializeReply_Returns_HttpResponseMessage()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter()
            {
                OnGetDefaultResponse = () =>
                {
                    return httpResponseMessage;
                },
                OnSerializeReply = (parameters, result, response) =>
                {
                    Assert.IsNotNull(response, "Response should not be null when SerializeReply is called");
                }
            };
            Message wcfMessage = formatter.SerializeReply(MessageVersion.None, parameters: new object[0], result: "result");
            Assert.IsNotNull(wcfMessage, "Returned WCF message cannot be null");
            HttpResponseMessage returnedHttpResponseMessage = wcfMessage.ToHttpResponseMessage();
            Assert.AreSame(httpResponseMessage, returnedHttpResponseMessage, "SerializeReply response message was not the one we returned.");
            Assert.IsTrue(((MockHttpMessageFormatter)formatter).WasSerializeReplyCalled, "SerializeReply on derived class was not called");
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.SerializeReply throws for null parameters argument")]
        public void SerializeReply_Null_Parameters_Throws()
        {
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            ExceptionAssert.ThrowsArgumentNull(
                "Null parameters should throw",
                "parameters",
                () =>
                {
                    formatter.SerializeReply(MessageVersion.None, /*parameters*/ null, /*result*/ "hello");
                });
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.SerializeReply accepts a null result argument")]
        public void SerializeReply_Null_Result_Allowed()
        {
            bool receivedNullResult = false;
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter()
            {
                OnSerializeReply = (parameters, result, response) =>
                {
                    receivedNullResult = (result == null);
                }
            };
            formatter.SerializeReply(MessageVersion.None, parameters: new object[0], result: null);
            Assert.IsTrue(receivedNullResult, "Null result did not make it through SerializeReply");
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.SerializeReply throws NotSupportedException if MessageVersion is not MessageVersion.None")]
        public void SerializeReply_MessageVersion_Not_None_Throws()
        {
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter();
            ExceptionAssert.Throws(
                typeof(NotSupportedException),
                "Illegal message version should throw",
                () =>
                {
                    formatter.SerializeReply(MessageVersion.Soap11, parameters: new object[0], result: "result");
                });
        }

        [TestMethod]
        [Description("IDispatchMessageFormatter.SerializeReply throws InvalidOperationException for null returned HttpResponseMessage")]
        public void SerializeReply_Null_HttpResponseMessage_Throws()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            IDispatchMessageFormatter formatter = new MockHttpMessageFormatter()
            {
                OnGetDefaultResponse = () =>
                {
                    return null;
                }
            };
            ExceptionAssert.ThrowsInvalidOperation(
                "Null returned from GetDefaultResponse should throw",
                () =>
                {
                    formatter.SerializeReply(MessageVersion.None, parameters: new object[0], result: "result");
                });
        }

        #endregion SerializeReply Tests
    }
}
