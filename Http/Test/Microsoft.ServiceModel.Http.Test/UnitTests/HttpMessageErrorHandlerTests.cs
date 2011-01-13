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
    public class HttpMessageErrorHandlerTests
    {
        #region HandleError Tests

        // Note: HandleError is not implemented in HttpMessageErrorHandler,
        // meaning there is nothing to test.

        #endregion HandleError Tests

        #region ProvideResponse Tests

        [TestMethod]
        [Description("HttpMessageErrorHandler.GetDefaultResponse throws for null response")]
        public void ProvideResponse_Null_GetDefaultResponse_Throws()
        {
            Exception error = new InvalidOperationException("problem");
            Message faultMessage = null;

            IErrorHandler errorHandler = new MockHttpMessageErrorHandler()
            {
                OnGetDefaultResponse = () => { return null; },
                OnProvideResponse = (e, m) => { Assert.Fail("ProvideResponse should not be called if throw occurs"); }
            };

            ExceptionAssert.Throws(
                typeof(InvalidOperationException),
                "Null from GetReponse should throw InvalidOperationException",
                () =>
                {
                    errorHandler.ProvideFault(error, MessageVersion.None, ref faultMessage);
                });
        }

        [TestMethod]
        [Description("HttpMessageErrorHandler.ProvideFault can return custom response message")]
        public void ProvideResponse_Returns_Custom_Response_Message()
        {
            Exception error = new InvalidOperationException("problem");
            HttpResponseMessage customResponseMessage = new HttpResponseMessage();
            Message faultMessage = null;

            IErrorHandler errorHandler = new MockHttpMessageErrorHandler()
            {
                OnGetDefaultResponse = () => { return customResponseMessage; },
                OnProvideResponse = (e, m) => { return; }
            };

            errorHandler.ProvideFault(error, MessageVersion.None, ref faultMessage);

            Assert.IsNotNull(faultMessage, "ProvideFault cannot yield null response");
            HttpResponseMessage responseMessage = faultMessage.ToHttpResponseMessage();
            Assert.AreSame(customResponseMessage, responseMessage, "ProvideFault should return custom message");
            Assert.IsTrue(((MockHttpMessageErrorHandler)errorHandler).WasProvideResponseCalled, "Derived class's ProvideFault was not called");
        }

        [TestMethod]
        [Description("HttpMessageErrorHandler.ProvideFault throws ArgumentNullException for null error argument")]
        public void ProvideResponse_Null_Error_Argument_Throws()
        {
            IErrorHandler errorHandler = new MockHttpMessageErrorHandler();
            Message faultMessage = null;
            ExceptionAssert.ThrowsArgumentNull(
                "Null error argument should throw ArgumentNull",
                "error",
                () =>
                {
                    errorHandler.ProvideFault(/*error*/ null, MessageVersion.None, ref faultMessage);
                });
        }

        #endregion ProvideResponse Tests
    }
}
