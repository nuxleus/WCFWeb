
namespace Microsoft.ServiceModel.Web.UnitTests
{
    using System;
    using System.Json;
    using System.Runtime.Serialization.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class JsonValueExtesnsionsTest
    {
        [TestMethod()]
        public void CreateFromTest()
        {
            JsonValue[] values =
            {
                AnyInstance.AnyJsonObject,
                AnyInstance.AnyJsonArray,
                AnyInstance.AnyJsonPrimitive,
                AnyInstance.DefaultJsonValue
            };

            foreach (JsonValue value in values)
            {
                Assert.AreSame(value, JsonValueExtensions.CreateFrom(value));
            }
        }

        [TestMethod()]
        public void CreateFromPrimitiveTest()
        {
            object[] values = 
            {
                AnyInstance.AnyBool,
                AnyInstance.AnyByte,
                AnyInstance.AnyChar,
                AnyInstance.AnyDateTime,
                AnyInstance.AnyDateTimeOffset,
                AnyInstance.AnyDecimal,
                AnyInstance.AnyDouble,
                AnyInstance.AnyFloat,
                AnyInstance.AnyGuid,
                AnyInstance.AnyLong,
                AnyInstance.AnySByte,
                AnyInstance.AnyShort,
                AnyInstance.AnyUInt,
                AnyInstance.AnyULong,
                AnyInstance.AnyUri,
                AnyInstance.AnyUShort,
                AnyInstance.AnyInt, 
                AnyInstance.AnyString, 

            };

            foreach (object value in values)
            {
                Type valueType = value.GetType();
                Console.WriteLine("Value: {0}, Type: {1}", value, valueType);
                Assert.AreEqual(value, JsonValueExtensions.CreateFrom(value).ReadAs(valueType), "Test failed on value of type: " + valueType);
            }
        }

        [TestMethod()]
        public void CreateFromComplexTest()
        {
            JsonValue target = JsonValueExtensions.CreateFrom(AnyInstance.AnyPerson);

            Assert.AreEqual(AnyInstance.AnyPerson.Name, (string)target["Name"]);
            Assert.AreEqual(AnyInstance.AnyPerson.Age, (int)target["Age"]);
            Assert.AreEqual(AnyInstance.AnyPerson.Address.City, (string)target.ValueOrDefault("Address", "City"));
        }
    }
}
