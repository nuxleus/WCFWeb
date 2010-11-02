﻿namespace System.Json.Test
{
    using System;
    using System.IO;
    using System.Json;
    using System.Reflection;
    using System.Security;
    using System.Security.Policy;
    using Microsoft.ServiceModel.Web.Test.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [Serializable]
    [TestClass]
    public class JsonValuePartialTrustTests
    {
        public static void AssertIsTrue(bool condition, string msg)
        {
            if (!condition)
            {
                throw new InvalidOperationException(msg);
            }
        }

        public static void AssertAreEqual(object obj1, object obj2, string msg)
        {
            if (obj1 == obj2)
            {
                return;
            }

            if (obj1 == null || obj2 == null || !obj1.Equals(obj2))
            {
                throw new InvalidOperationException(string.Format("[{0}, {2}] and [{1}, {3}] expected to be equal. {4}", obj1, obj2, obj1.GetType().Name, obj2.GetType().Name, msg));
            }
        }

        [TestMethod]
        public void RunNonDynamicTest()
        {
            RunInPartialTrust(this.NonDynamicTest);
        }

        [TestMethod]
        public void RunDynamicTest()
        {
            RunInPartialTrust(this.DynamicTest);
        }

        public void NonDynamicTest()
        {
            int seed = GetRandomSeed();
            Console.WriteLine("Seed: {0}", seed);
            Random rndGen = new Random(seed);

            AssertIsTrue(Assembly.GetExecutingAssembly().IsFullyTrusted == false, "Executing assembly not expected to be fully trusted!");

            Person person = new Person(rndGen);
            Person person2 = new Person(rndGen);

            person.AddFriends(3, rndGen);
            person2.AddFriends(3, rndGen);
            
            JsonValue jo = JsonValue.CreateFrom(person);
            JsonValue jo2 = JsonValue.CreateFrom(person2);

            AssertAreEqual(person.Address.City, jo["Address"]["City"].ReadAs<string>(), "Address.City");
            AssertAreEqual(person.Friends[1].Age, jo["Friends"][1]["Age"].ReadAs<int>(), "Friends[1].Age");

            string newCityName = "Bellevue";

            jo["Address"]["City"] = newCityName;
            AssertAreEqual(newCityName, (string)jo["Address"]["City"], "Address.City2");

            jo["Friends"][1] = jo2;
            AssertAreEqual(person2.Age, (int)jo["Friends"][1]["Age"], "Friends[1].Age2");

            AssertAreEqual(person2.Address.City, jo.ValueOrDefault("Friends").ValueOrDefault(1).ValueOrDefault("Address").ValueOrDefault("City").ReadAs<string>(), "Address.City3");
            AssertAreEqual(person2.Age, (int)jo.ValueOrDefault("Friends").ValueOrDefault(1).ValueOrDefault("Age"), "Friends[1].Age3");

            AssertAreEqual(person2.Address.City, jo.ValueOrDefault("Friends", 1, "Address", "City").ReadAs<string>(), "Address.City3");
            AssertAreEqual(person2.Age, (int)jo.ValueOrDefault("Friends", 1, "Age"), "Friends[1].Age3");

            int newAge = 42;
            JsonValue ageValue = jo["Friends"][1]["Age"] = newAge;
            AssertAreEqual(newAge, (int)ageValue, "Friends[1].Age4");
        }

        public void DynamicTest()
        {
            int seed = GetRandomSeed();
            Console.WriteLine("Seed: {0}", seed);
            Random rndGen = new Random(seed);

            AssertIsTrue(Assembly.GetExecutingAssembly().IsFullyTrusted == false, "Executing assembly not expected to be fully trusted!");

            Person person = new Person(rndGen);
            person.AddFriends(1, rndGen);

            dynamic jo = JsonValue.CreateFrom(person);

            AssertAreEqual(jo.Friends[0].ToString(), jo.Friends[0].ToString(), "Friends[0].ToString()");

            AssertAreEqual(person.Address.City, jo.Address.City.ReadAs<string>(), "Address.City");
            AssertAreEqual(person.Friends[0].Age, (int)jo.Friends[0].Age, "Friends[0].Age");

            string newCityName = "Bellevue";

            jo.Address.City = newCityName;
            AssertAreEqual(newCityName, (string)jo.Address.City, "Address.City2");

            AssertAreEqual(person.Friends[0].Address.City, jo.ValueOrDefault("Friends").ValueOrDefault(0).ValueOrDefault("Address").ValueOrDefault("City").ReadAs<string>(), "Friends[0].Address.City");
            AssertAreEqual(person.Friends[0].Age, (int)jo.ValueOrDefault("Friends").ValueOrDefault(0).ValueOrDefault("Age"), "Friends[0].Age2");

            AssertAreEqual(person.Friends[0].Address.City, jo.ValueOrDefault("Friends", 0, "Address", "City").ReadAs<string>(), "Friends[0].Address.City");
            AssertAreEqual(person.Friends[0].Age, (int)jo.ValueOrDefault("Friends", 0, "Age"), "Friends[0].Age2");

            int newAge = 42;
            JsonValue ageValue = jo.Friends[0].Age = newAge;
            AssertAreEqual(newAge, (int)ageValue, "Friends[0].Age3");

            AssertIsTrue(jo.NonExistentProperty is JsonValue, "Expected a JsonValue");
            AssertIsTrue(jo.NonExistentProperty.JsonType == JsonType.Default, "Expected default JsonValue");
        }

        internal static void RunInPartialTrust(CrossAppDomainDelegate testMethod)
        {
            Assert.IsTrue(Assembly.GetExecutingAssembly().IsFullyTrusted);

            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PermissionSet perms = PermissionsHelper.InternetZone;
            AppDomain domain = AppDomain.CreateDomain("PartialTrustSandBox", null, setup, perms);

            domain.DoCallBack(testMethod);
        }

        private static int GetRandomSeed()
        {
            DateTime now = DateTime.Now;
            return (now.Year * 10000) + (now.Month * 100) + now.Day;
        }

        internal static class PermissionsHelper
        {
            private static PermissionSet internetZone;

            public static PermissionSet InternetZone
            {
                get
                {
                    if (internetZone == null)
                    {
                        Evidence evidence = new Evidence();
                        evidence.AddHostEvidence(new Zone(SecurityZone.Internet));

                        internetZone = SecurityManager.GetStandardSandbox(evidence);
                    }

                    return internetZone;
                }
            }
        }
    }
}
