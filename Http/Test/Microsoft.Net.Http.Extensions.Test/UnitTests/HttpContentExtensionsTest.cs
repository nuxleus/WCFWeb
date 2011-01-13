// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.Net.Http.Extensions.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpContentExtensionsTest
    {
        private const string CustomerJson = "{\"CustomerID\":1}";
        private const string CustomerXml = "<Customer><CustomerID>1</CustomerID></Customer>";


        [TestMethod]
        public void WhenConvertingUsingFormattersAndCorrectFormatterIsPresentThenItIsCalled()
        {
            var formatter = new MockContentFormatter(new MediaTypeHeaderValue("application/foo"));
            var content = new StringContent("", Encoding.UTF8, "application/foo");
            content.ReadAsObject<Customer>(formatter);
            Assert.IsTrue(formatter.ReadFromStreamCalled);
        }

        [TestMethod]
        public void WhenConvertingUsingFormattersAndNoFormatterIsPresentAndContentTypeIsXmlThenInstanceIsReturned()
        {
            var content = new StringContent(CustomerXml,Encoding.UTF8, "application/xml");
            var customer = content.ReadAsObject<Customer>();
            Assert.AreEqual(1, customer.CustomerID);
        }

        [TestMethod]
        public void WhenConvertingUsingFormattersAndNoFormatterIsPresentAndContentTypeIsJsonThenInstanceIsReturned()
        {
            var content = new StringContent(CustomerJson, Encoding.UTF8, "application/json");
            var customer = content.ReadAsObject<Customer>();
            Assert.AreEqual(1, customer.CustomerID);
        }


    }
}
