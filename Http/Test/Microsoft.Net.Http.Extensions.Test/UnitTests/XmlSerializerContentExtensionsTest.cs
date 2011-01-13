// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Xml.Serialization;
    using Microsoft.Net.Http.Extensions.Test.UnitTests.Mocks;
    using Microsoft.Xml.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XmlSerializerContentExtensionsTest
    {
        private const string CustomerXml = "<Customer><CustomerID>1</CustomerID></Customer>";

        [TestMethod]
        public void WhenConvertingToDataContractThenInstanceIsReturned()
        {
            var content = new StringContent(CustomerXml);
            var customer = content.ReadAsXmlSerializable<Customer>();
            Assert.AreEqual(1, customer.CustomerID);
        }

        [TestMethod]
        public void WhenConvertingToDataContractAndPassingExtraTypesThenInstanceIsReturned()
        {
            var customer = new CustomerWithItems() { CustomerID = 1 };
            customer.Items = new List<object> { new ItemTypeA { Name = "An Item" } };
            var serializer = new XmlSerializer(typeof(CustomerWithItems), new Type[] { typeof(ItemTypeA) });
            var stream = new MemoryStream();
            serializer.Serialize(stream, customer);
            stream.Position = 0;
            customer = (CustomerWithItems)serializer.Deserialize(stream);
            Assert.AreEqual("An Item", ((ItemTypeA)customer.Items[0]).Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))] //fake is just a stub which throws an exception...hacky but works
        public void WhenConvertingToDataContractAndPassingASerializerThenTheSerializerIsInvoked()
        {
            var serializer = new MockXmlSerializer(typeof(Customer));
            var content = new StringContent(CustomerXml);
            var customer = content.ReadAsXmlSerializable<Customer>(serializer);
        }

        [TestMethod]
        public void WhenConvertingToContentThenContentIsReturned()
        {
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingXmlSerializer();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingToContentThenXmlStreamHasCorrectData()
        {
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingXmlSerializer();
            var reader = new StreamReader(content.ContentReadStream);
            var xml = reader.ReadToEnd();
            Assert.IsTrue(xml.Contains("<CustomerID>1</CustomerID>"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenConvertingToContentPassingASerializerThenTheSerializerIsInvoked()
        {
            var serializer = new MockXmlSerializer(typeof(Customer));
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingXmlSerializer(serializer);
        }


    }
}
