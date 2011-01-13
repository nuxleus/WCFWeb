// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Microsoft.Net.Http.Extensions.Test.Mocks;
    using Microsoft.Net.Http.Extensions.Test.UnitTests;
    using Microsoft.Runtime.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataContractContentExtensionsTest
    {
        private const string CustomerXml = "<Customer xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.Net.Http.Extensions.Test.UnitTests\"><CustomerID>1</CustomerID></Customer>";

        [TestMethod]
        public void WhenConvertingToDataContractThenInstanceIsReturned()
        {
            var content = new StringContent(CustomerXml);
            var customer = content.ReadAsDataContract<Customer>();
            Assert.AreEqual(1, customer.CustomerID);
        }

        [TestMethod]
        public void WhenConvertingToDataContractAndPassingExtraTypesThenInstanceIsReturned()
        {
            var customer = new CustomerWithItems() {CustomerID = 1};
            customer.Items = new List<object> {new ItemTypeA {Name = "An Item"}};
            var serializer = new DataContractSerializer(typeof (CustomerWithItems), new List<Type>{typeof(ItemTypeA)});
            var stream = new MemoryStream();
            serializer.WriteObject(stream, customer);
            stream.Position = 0;
            customer = (CustomerWithItems) serializer.ReadObject(stream);
            Assert.AreEqual("An Item", ((ItemTypeA)customer.Items[0]).Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))] //fake is just a stub which throws an exception...hacky but works
        public void WhenConvertingToDataContractAndPassingASerializerThenTheSerializerIsInvoked()
        {
            var surrogate = new FakeDataContractSurrogate();
            var serializer = GetSerializerUsingSurrogate(surrogate);
            var content = new StringContent(CustomerXml);
            var customer = content.ReadAsDataContract<Customer>(serializer);
        }

        [TestMethod]
        public void WhenConvertingToContentThenContentIsReturned()
        {
            var customer = new Customer {CustomerID = 1};
            var content = customer.ToContentUsingDataContractSerializer();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingToContentThenXmlStreamHasCorrectData()
        {
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingDataContractSerializer();
            var reader = new StreamReader(content.ContentReadStream);
            var xml = reader.ReadToEnd();
            Assert.IsTrue(xml.Contains("<CustomerID>1</CustomerID>"));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void WhenConvertingToContentPassingASerializerThenTheSerializerIsInvoked()
        {
            var surrogate = new FakeDataContractSurrogate();
            var serializer = GetSerializerUsingSurrogate(surrogate);
            var customer = new Customer {CustomerID = 1};
            var content = customer.ToContentUsingDataContractSerializer(serializer);
        }

        private DataContractSerializer GetSerializerUsingSurrogate(FakeDataContractSurrogate surrogate)
        {
            return new DataContractSerializer(typeof(Customer), null, int.MaxValue, false, false, surrogate);
        }
    }
}
