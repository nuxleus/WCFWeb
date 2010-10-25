// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System.ServiceModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryCompositionAttributeTests
    {
        [TestMethod]
        public void TestDefaultCtor()
        {
            QueryCompositionAttribute attr = new QueryCompositionAttribute();
            Assert.IsTrue(attr.Enabled);
            Assert.IsNull(attr.QueryComposerType);
        }

        [TestMethod]
        public void TestCtor()
        {
            QueryCompositionAttribute attr = new QueryCompositionAttribute(typeof(UrlQueryComposer));
            Assert.IsTrue(attr.Enabled);
            Assert.IsNotNull(attr.QueryComposerType);
            Assert.AreEqual(attr.QueryComposerType, typeof(UrlQueryComposer));
        }    
    }
}
