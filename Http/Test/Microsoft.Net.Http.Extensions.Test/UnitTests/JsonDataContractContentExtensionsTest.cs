// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using Microsoft.Net.Http.Extensions.Test.Mocks;
    using Microsoft.Runtime.Serialization.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass]
    public class JsonDataContractContentExtensionsTest
    {
        private const string CustomerJson = "{\"CustomerID\":1}";

        [TestMethod]
        public void WhenConvertingToDataContractThenInstanceIsReturned()
        {
            var content = new StringContent(CustomerJson);
            var customer = content.ReadAsJsonDataContract<Customer>();
            Assert.AreEqual(1, customer.CustomerID);
        }

        [TestMethod]
        public void WhenConvertingToDataContractAndPassingExtraTypesThenInstanceIsReturned()
        {
            var customer = new CustomerWithItems() { CustomerID = 1 };
            customer.Items = new List<object> { new ItemTypeA { Name = "An Item" } };
            var serializer = new DataContractJsonSerializer(typeof(CustomerWithItems), new List<Type> { typeof(ItemTypeA) });
            var stream = new MemoryStream();
            serializer.WriteObject(stream, customer);
            stream.Position = 0;
            customer = (CustomerWithItems)serializer.ReadObject(stream);
            Assert.AreEqual("An Item", ((ItemTypeA)customer.Items[0]).Name);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))] //fake is just a stub which throws an exception...hacky but works
        public void WhenConvertingToDataContractAndPassingASerializerThenTheSerializerIsInvoked()
        {
            var surrogate = new FakeDataContractSurrogate();
            var serializer = GetSerializerUsingSurrogate(surrogate);
            var content = new StringContent(CustomerJson);
            var customer = content.ReadAsJsonDataContract<Customer>(serializer);
        }

        private DataContractJsonSerializer GetSerializerUsingSurrogate(FakeDataContractSurrogate surrogate)
        {
            return new DataContractJsonSerializer(typeof(Customer), null, int.MaxValue, false, surrogate, false);
        }

        [TestMethod]
        public void WhenConvertingToContentThenContentIsReturned()
        {
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingDataContractJsonSerializer();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingToContentThenXmlStreamHasCorrectData()
        {
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingDataContractJsonSerializer();
            var reader = new StreamReader(content.ContentReadStream);
            var json = reader.ReadToEnd();
            Assert.IsTrue(json.Contains(CustomerJson));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void WhenConvertingToContentPassingASerializerThenTheSerializerIsInvoked()
        {
            var surrogate = new FakeDataContractSurrogate();
            var serializer = GetSerializerUsingSurrogate(surrogate);
            var customer = new Customer { CustomerID = 1 };
            var content = customer.ToContentUsingDataContractJsonSerializer(serializer);
        }
    }
}
