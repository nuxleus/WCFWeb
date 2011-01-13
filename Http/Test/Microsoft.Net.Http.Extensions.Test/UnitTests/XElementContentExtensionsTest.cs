// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System.IO;
    using System.Net.Http;
    using System.Xml.Linq;
    using Microsoft.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XElementContentExtensionsTest
    {
        static HttpContent GetContent()
        {
            return new StringContent("<Foo/>");
        }

        [TestMethod]
        public void WhenConvertingFromContentToXElementThenXElementIsReturned()
        {
            var content = GetContent();
            var element = content.ReadAsXElement();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingFromContentToXElementThenResultElementContainsXmlThatWasPassed()
        {
            var content = GetContent();
            var element = content.ReadAsXElement();
            Assert.AreEqual("Foo", element.Name);
        }

        [TestMethod]
        public void WhenConvertingFromXElementToContentThenContentIsReturned()
        {
            var element = new XElement("Foo");
            var content = element.ToContent();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingFromXElementToContentThenStreamContainsProperXml()
        {
            var element = new XElement("Foo");
            var content = element.ToContent();
            var reader = new StreamReader(content.ContentReadStream);
            var xml = reader.ReadToEnd();
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Foo />",xml);
        }


    }
}
