
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

        [TestMethod]
        public void CreateFromDynamicSimpleTest()
        {
            JsonValue target;

            target = JsonValueExtensions.CreateFrom(AnyInstance.AnyDynamic);
            Assert.IsNotNull(target);

            string expected = "{\"Name\":\"Bill Gates\",\"Age\":21,\"Grades\":[\"F\",\"B-\",\"C\"]}";
            dynamic obj = new TestDynamicObject();
            obj.Name = "Bill Gates";
            obj.Age = 21;
            obj.Grades = new[] { "F", "B-", "C" };

            target = JsonValueExtensions.CreateFrom(obj);
            Assert.AreEqual<string>(expected, target.ToString());

            target = JsonValueExtensions.CreateFrom(new TestDynamicObject());
            Assert.AreEqual<string>("{}", target.ToString());
        }

        [TestMethod]
        public void CreateFromDynamicComplextTest()
        {
            JsonValue target;
            Person person = AnyInstance.AnyPerson;
            dynamic dyn = TestDynamicObject.CreatePersonAsDynamic(person);

            target = JsonValueExtensions.CreateFrom(dyn);
            Person jvPerson = target.ReadAsComplex<Person>();
            Assert.AreEqual(person.ToString(), jvPerson.ToString());

            Person p1 = Person.CreateSample();
            Person p2 = Person.CreateSample();

            p2.Name += "__2";
            p2.Age += 10;
            p2.Address.City += "__2";

            Person[] friends = new Person[] { p1, p2 };
            target = JsonValueExtensions.CreateFrom(friends);
            Person[] personArr = target.ReadAsComplex<Person[]>();
            Assert.AreEqual<int>(friends.Length, personArr.Length);
            Assert.AreEqual<string>(friends[0].ToString(), personArr[0].ToString());
            Assert.AreEqual<string>(friends[1].ToString(), personArr[1].ToString());
        }

        [TestMethod]
        public void CreateFromNestedDynamicTest()
        {
            JsonValue target;
            string expected = "{\"Name\":\"Root\",\"Level1\":{\"Name\":\"Level1\",\"Level2\":{\"Name\":\"Level2\"}}}";
            dynamic dyn = new TestDynamicObject();
            dyn.Name = "Root";
            dyn.Level1 = new TestDynamicObject();
            dyn.Level1.Name = "Level1";
            dyn.Level1.Level2 = new TestDynamicObject();
            dyn.Level1.Level2.Name = "Level2";

            target = JsonValueExtensions.CreateFrom(dyn);
            Assert.AreEqual<string>(expected, target.ToString());
        }

        [TestMethod]
        public void CreateFromDynamicJVTest()
        {
            JsonValue target;

            dynamic[] values = new dynamic[]
            {
                AnyInstance.AnyJsonArray,
                AnyInstance.AnyJsonObject,
                AnyInstance.AnyJsonPrimitive,
                AnyInstance.DefaultJsonValue
            };

            foreach (dynamic dyn in values)
            {
                target = JsonValueExtensions.CreateFrom(dyn);
                Assert.AreSame(dyn, target);
            }
        }
    }
}
