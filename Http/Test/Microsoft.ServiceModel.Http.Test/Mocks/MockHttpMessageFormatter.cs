// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockHttpMessageFormatter : HttpMessageFormatter
    {
        public bool WasDeserializeRequestCalled { get; set; }
        public bool WasSerializeReplyCalled { get; set; }
        public Action<HttpRequestMessage, object[]> OnDeserializeRequest { get; set; }
        public Action<object[], object, HttpResponseMessage> OnSerializeReply { get; set; }
        public Func<HttpResponseMessage> OnGetDefaultResponse { get; set; }

        protected override void DeserializeRequest(HttpRequestMessage message, object[] parameters)
        {
            this.WasDeserializeRequestCalled = true;
            if (this.OnDeserializeRequest != null)
            {
                this.OnDeserializeRequest(message, parameters);
                return;
            }
            Assert.Fail("Register OnDeserializeRequest first");
        }

        protected override HttpResponseMessage GetDefaultResponse()
        {
            if (this.OnGetDefaultResponse != null)
            {
                return this.OnGetDefaultResponse();
            }
            return base.GetDefaultResponse();
        }

        protected override void SerializeReply(object[] parameters, object result, HttpResponseMessage response)
        {
            this.WasSerializeReplyCalled = true;

            if (this.OnSerializeReply != null)
            {
                this.OnSerializeReply(parameters, result, response);
                return;
            }
            Assert.Fail("Register a OnSerializeReply first");
        }
    }
}
