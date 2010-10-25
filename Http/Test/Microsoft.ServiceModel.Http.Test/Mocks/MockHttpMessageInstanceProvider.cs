// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockHttpMessageInstanceProvider : HttpMessageInstanceProvider
    {
        public bool WasGetInstanceCalled { get; set; }
        public Func<InstanceContext, HttpRequestMessage, object> OnGetInstance { get; set; }

        protected override object GetInstance(InstanceContext instanceContext, HttpRequestMessage request)
        {
            this.WasGetInstanceCalled = true;
            if (this.OnGetInstance != null)
            {
                return this.OnGetInstance(instanceContext, request);
            }
            Assert.Fail("Register OnGetInstance before using this mock.");
            return null;
        }

        public override object GetInstance(InstanceContext instanceContext)
        {
            throw new NotImplementedException();
        }

        public override void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            throw new NotImplementedException();
        }
    }
}
