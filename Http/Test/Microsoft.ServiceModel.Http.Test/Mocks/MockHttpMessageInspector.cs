// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockHttpMessageInspector : HttpMessageInspector
    {
        public Func<HttpRequestMessage, object> OnAfterReceiveRequest { get; set; }
        public Action<HttpResponseMessage, object> OnBeforeSendReply { get; set; }
        public bool WasAfterReceiveRequestCalled { get; set; }
        public bool WasBeforeSendReplyCalled { get; set; }

        protected override object AfterReceiveRequest(HttpRequestMessage request)
        {
            this.WasAfterReceiveRequestCalled = true;
            if (this.OnAfterReceiveRequest != null)
            {
                return this.OnAfterReceiveRequest(request);
            }
            Assert.Fail("Set the OnAfterReceiveRequest before using this mock.");
            return null;
        }

        protected override void BeforeSendReply(HttpResponseMessage reply, object correlationState)
        {
            this.WasBeforeSendReplyCalled = true;
            if (this.OnBeforeSendReply != null)
            {
                this.OnBeforeSendReply(reply, correlationState);
                return;
            }
            Assert.Fail("Set the OnBeforeSendReply before using this mock.");
        }
    }
}
