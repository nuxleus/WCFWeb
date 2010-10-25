// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using Microsoft.ServiceModel.Http.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryCompositionOperationInvokerTests
    {
        [TestMethod]
        public void TestCtor()
        {
            MethodInfo mi = typeof(QueryService).GetMethod("GetCustomers");
            QueryCompositionOperationInvoker invoker = new QueryCompositionOperationInvoker(new MockOperationInvoker(mi), new UrlQueryComposer());
            Assert.AreEqual(0, invoker.AllocateInputs().ToList<object>().Count);
            Assert.IsTrue(invoker.IsSynchronous);
        }
    }
}
