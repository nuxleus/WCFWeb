

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Microsoft.Xml;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XmlDocumentExtentionsTest
    {
        private const string Xml="<Foo />";

        [TestMethod]
        public void WhenConvertingFromXmlDocumentToContentTheContentIsReturned()
        {
            var document = this.GetDocument();
            var content = document.ToContent();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingFromXmlDocumentToContentThenContentContainsProperXml()
        {
            var document = this.GetDocument();
            var content = document.ToContent();
            var reader = new StreamReader(content.ContentReadStream);
            var xml = reader.ReadToEnd();
            Assert.AreEqual(Xml, xml);
        }

        [TestMethod]
        public void WhenConvertingFromContentToXmlDocumentThenDocumentContainsProperXml()
        {
            var content = new StringContent(Xml);
            var document = content.ReadAsXmlDocument();
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);
            document.Save(writer);
            var xml = builder.ToString();
            Assert.IsTrue(xml.Contains(Xml));
        }

        private XmlDocument GetDocument()
        {
            var document = new XmlDocument();
            document.LoadXml(Xml);
            return document;
        }
    }
}
