// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Mock HttpMessageErrorHandler that tests ability to subclass and probes return paths.
    /// </summary>
    public class MockHttpMessageErrorHandler : HttpMessageErrorHandler
    {
        public bool WasProvideResponseCalled { get; set; }
        public Action<Exception, HttpResponseMessage> OnProvideResponse { get; set; }
        public Func<HttpResponseMessage> OnGetDefaultResponse { get; set; }

        public override bool HandleError(Exception error)
        {
            // Our Http class does not override this.  Nothing to test.
            throw new NotImplementedException();
        }

        protected override HttpResponseMessage GetDefaultResponse()
        {
            if (this.OnGetDefaultResponse != null)
            {
                HttpResponseMessage response = this.OnGetDefaultResponse();
                return response;
            }
            return base.GetDefaultResponse();
        }

        protected override void ProvideResponse(Exception error, HttpResponseMessage response)
        {
            this.WasProvideResponseCalled = true;

            if (this.OnProvideResponse != null)
            {
                this.OnProvideResponse(error, response);
            }
            else
            {
                Assert.Fail("Register the OnProvideResponse to use this mock.");
            }
        }
    }
}
