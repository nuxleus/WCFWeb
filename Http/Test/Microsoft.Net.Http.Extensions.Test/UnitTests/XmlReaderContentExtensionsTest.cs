namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xml;

    [TestClass]
    public class XmlReaderContentExtensionsTest
    {
        private const string Xml = "<Foo />";

        [TestMethod]
        public void WhenConvertingFromContentToXmlReaderThenReaderContainsProperXml()
        {
            var content = new StringContent(Xml);
            var reader = content.ReadAsXmlReader();
            var xml = reader.ReadOuterXml();
        }
    }
}
