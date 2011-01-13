// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Net.Http.Extensions.Test.UnitTests
{
    using System.Json;
    using System.Net.Http;
    using Microsoft.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonValueContentExtensionsTest
    {
        private const string CustomerJson = "{\"CustomerID\":1}";

        static HttpContent GetContent()
        {
            var jsonValue = new JsonObject();
            jsonValue["Foo"] = "Bar";
            return jsonValue.ToContent();
        }

        [TestMethod]
        public void WhenConvertingToStreamContentThenStreamContentIsReturned()
        {
            var content = GetContent();
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void WhenConvertingToStreamContentThenResultStreamContainsJsonThatWasPassed()
        {
            var content = GetContent();
            var jsonValue = JsonValue.Load(content.ContentReadStream);
            var value = jsonValue["Foo"].ReadAs<string>();
            Assert.AreEqual("Bar", value);
        }

        [TestMethod]
        public void WhenConvertingToJsonValueThenJsonValueIsReturned()
        {
            var content = new StringContent(CustomerJson);
            var jsonValue = content.ReadAsJsonValue();
            Assert.AreEqual(1, jsonValue["CustomerID"].ReadAs<int>());
        }
    }
}
