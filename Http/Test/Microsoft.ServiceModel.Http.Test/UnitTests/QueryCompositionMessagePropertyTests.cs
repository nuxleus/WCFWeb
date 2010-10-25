// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.ServiceModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryCompositionMessagePropertyTests
    {
        string address = "http://localhost/service1";

        [TestMethod]
        public void TestCtor()
        {
            QueryCompositionMessageProperty property = new QueryCompositionMessageProperty(address);
            Assert.AreEqual(address, property.RequestUri);
        }

        [TestMethod]
        public void TestSetter()
        {
            string address2 = "http://localhost/service2";
            QueryCompositionMessageProperty property = new QueryCompositionMessageProperty(address);
            property.RequestUri = address2;
            Assert.AreEqual(address2, property.RequestUri);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetterNull()
        {
            QueryCompositionMessageProperty property = new QueryCompositionMessageProperty(address);
            property.RequestUri = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetterEmpty()
        {
            QueryCompositionMessageProperty property = new QueryCompositionMessageProperty(address);
            property.RequestUri = "";
        }

    }
}
