namespace Microsoft.ServiceModel.Web.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Json;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class JsonDefaultTest
    {
        [TestMethod()]
        public void PropertiesTest()
        {
            JsonValue target = AnyInstance.DefaultJsonValue;

            Assert.AreEqual(JsonType.Default, target.JsonType);
            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(false, target.ContainsKey("hello"));
            Assert.AreEqual(false, target.ContainsKey(string.Empty));
        }

        [TestMethod()]
        public void SaveTest()
        {
            JsonValue target = AnyInstance.DefaultJsonValue;
            using (MemoryStream ms = new MemoryStream())
            {
                ExceptionTestHelper.ExpectException<InvalidOperationException>(delegate { target.Save(ms); });
            }
        }

        [TestMethod()]
        public void ToStringTest()
        {
            JsonValue target;
            
            target = AnyInstance.DefaultJsonValue;
            Assert.AreEqual(target.ToString(), "Default");
        }

        [TestMethod()]
        public void ReadAsTests()
        {
            JsonValue target = AnyInstance.DefaultJsonValue;

            ExceptionTestHelper.ExpectException<NotSupportedException>(delegate { target.ReadAs<bool>(); });
            ExceptionTestHelper.ExpectException<NotSupportedException>(delegate { target.ReadAs<string>(); });
            ExceptionTestHelper.ExpectException<NotSupportedException>(delegate { target.ReadAs<JsonObject>(); });

            bool boolValue;
            string stringValue;
            JsonObject objValue;

            Assert.IsFalse(target.TryReadAs<bool>(out boolValue));
            Assert.IsFalse(target.TryReadAs<string>(out stringValue));
            Assert.IsFalse(target.TryReadAs<JsonObject>(out objValue));
        }

        [TestMethod()]
        public void ItemTests()
        {
            JsonValue target = AnyInstance.DefaultJsonValue;

            ExceptionTestHelper.ExpectException<InvalidOperationException>(delegate { var v = target["MissingProperty"]; });
            ExceptionTestHelper.ExpectException<InvalidOperationException>(delegate { target["NewProperty"] = AnyInstance.AnyJsonValue1; });
        }

        [TestMethod()]
        public void DynamicItemTests() 
        {
            dynamic target = AnyInstance.DefaultJsonValue;

            var getByKey = target["SomeKey"];
            Assert.AreSame(getByKey, AnyInstance.DefaultJsonValue);

            var getByIndex = target[10];
            Assert.AreSame(getByIndex, AnyInstance.DefaultJsonValue);

            ExceptionTestHelper.ExpectException<InvalidOperationException>(delegate { target["SomeKey"] = AnyInstance.AnyJsonObject; });
            ExceptionTestHelper.ExpectException<InvalidOperationException>(delegate { target[10] = AnyInstance.AnyJsonObject; });
        }

        [TestMethod()]
        public void DefaultConcatTest()
        {
            JsonValue jv = JsonValue.CreateFrom(AnyInstance.AnyPerson);
            dynamic target = JsonValue.CreateFrom(AnyInstance.AnyPerson);
            Person person = AnyInstance.AnyPerson;

            Assert.AreEqual(JsonType.Default, target.Friends[100000].Name.JsonType);
            Assert.AreEqual(JsonType.Default, target.Friends[0].Age.Minutes.JsonType);

            JsonValue jv1 = target.MissingProperty as JsonValue;
            Assert.IsNotNull(jv1);

            JsonValue jv2 = target.MissingProperty1.MissingProperty2 as JsonValue;
            Assert.IsNotNull(jv2);

            Assert.AreSame(jv1, jv2);
            Assert.AreSame(target.Person.Name.MissingProperty, AnyInstance.DefaultJsonValue);
        }

        [TestMethod()]
        public void CastingDefaultValueTest()
        {
            JsonValue jv = AnyInstance.DefaultJsonValue;
            dynamic d = jv;

            ExceptionTestHelper.ExpectException<InvalidCastException>(delegate { float p = (float)d; });
            ExceptionTestHelper.ExpectException<InvalidCastException>(delegate { byte p = (byte)d; });
            ExceptionTestHelper.ExpectException<InvalidCastException>(delegate { int p = (int)d; });

            Assert.IsNull((string)d);
        }
    }
}
