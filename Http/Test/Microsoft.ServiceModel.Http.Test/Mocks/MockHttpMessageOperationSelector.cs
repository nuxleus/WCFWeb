// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Http.Test.Mocks
{
    using System;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Mock HttpMessageOperationSelector that tests ability to subclass and probes return paths.
    /// </summary>
    public class MockHttpMessageOperationSelector : HttpMessageOperationSelector
    {
        public bool WasSelectOperationCalled { get; set; }
        public Func<HttpRequestMessage, string> OnSelectOperation { get; set; }

        protected override string SelectOperation(HttpRequestMessage message)
        {
            this.WasSelectOperationCalled = true;
            if (this.OnSelectOperation != null)
            {
                return this.OnSelectOperation(message);
            }
            Assert.Fail("Set the OnSelectOperation before using this mock.");
            return null;
        }
    }
}
