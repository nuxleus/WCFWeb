﻿namespace System.Json.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Json;
    using System.Linq;
    using Microsoft.Silverlight.Cdf.Test.Common.Utility;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JLinqUsageTests
    {
        static TextWriter Log
        {
            get { return Console.Out; }
        }

        [TestMethod]
        public void JLinqSimpleCreationQueryTest()
        {
            int seed = 1;
            Random rndGen = new Random(seed);

            JsonArray sourceJson = new JsonArray
            {
                new JsonObject { { "Name", "Alex" }, { "Age", 18 }, { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) } },
                new JsonObject { { "Name", "Joe" }, { "Age", 19 }, { "Birthday", DateTime.MinValue } },
                new JsonObject { { "Name", "Chris" }, { "Age", 20 }, { "Birthday", DateTime.Now } },
                new JsonObject { { "Name", "Jeff" }, { "Age", 21 }, { "Birthday", DateTime.MaxValue } },
                new JsonObject { { "Name", "Carlos" }, { "Age", 22 }, { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) } },
                new JsonObject { { "Name", "Mohammad" }, { "Age", 23 }, { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) } },
                new JsonObject { { "Name", "Sara" }, { "Age", 24 }, { "Birthday", new DateTime(1998, 3, 20) } },
                new JsonObject { { "Name", "Tomasz" }, { "Age", 25 }, { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) } },
                new JsonObject { { "Name", "Suwat" }, { "Age", 26 }, { "Birthday", new DateTime(1500, 12, 20) } },
                new JsonObject { { "Name", "Eugene" }, { "Age", 27 }, { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) } }
            };

            var adults = from JsonValue adult in sourceJson
                         where (int)adult["Age"] > 21
                         select adult;
            Log.WriteLine("Team contains: ");
            int count = 0;
            foreach (JsonValue adult in adults)
            {
                count++;
                Log.WriteLine((string)adult["Name"]);
            }

            if (count != 6)
            {
                Assert.Fail("There should be 6 adults, but JLinq query only located " + count);
            }
        }

        [TestMethod]
        public void JLinqSimpleQueryTest()
        {
            JsonArray sourceJson = this.CreateArrayOfPeople();

            var adults = from JsonValue adult in sourceJson
                         where (int)adult["Age"] > 21
                         select adult;
            Log.WriteLine("Team contains: ");
            int count = 0;
            foreach (JsonValue adult in adults)
            {
                count++;
                Log.WriteLine((string)adult["Name"]);
            }

            if (count != 6)
            {
                Assert.Fail("There should be 6 adults, but JLinq query only located " + count);
            }
        }

        [TestMethod]
        public void JLinqDeepQueryTest()
        {
            int seed = 1;

            JsonArray mixedOrderJsonObj;
            JsonArray myJsonObj = SpecialJsonValueHelper.CreateDeepLevelJsonValuePair(seed, out mixedOrderJsonObj, Log);

            if (myJsonObj != null && mixedOrderJsonObj != null)
            {
                bool retValue = true;

                var dict = new Dictionary<string, int>
                {
                    { "myArray", 1 }, 
                    { "myArrayLevel2", 2 }, 
                    { "myArrayLevel3", 3 }, 
                    { "myArrayLevel4", 4 }, 
                    { "myArrayLevel5", 5 }, 
                    { "myArrayLevel6", 6 }, 
                    { "myArrayLevel7", 7 },
                    { "myBool", 8 }, 
                    { "myByte", 9 }, 
                    { "myDatetime", 10 },
                    { "myDateTimeOffset", 11 },
                    { "myDecimal", 12 },
                    { "myDouble", 13 }, 
                    { "myInt16", 14 }, 
                    { "myInt32", 15 }, 
                    { "myInt64", 16 }, 
                    { "mySByte", 17 }, 
                    { "mySingle", 18 },
                    { "myString", 19 },
                    { "myUInt16", 20 },
                    { "myUInt32", 21 },
                    { "myUInt64", 22 }
                };

                foreach (string name in dict.Keys)
                {
                    if (!this.InternalVerificationViaLinqQuery(myJsonObj, name, dict[name]))
                    {
                        retValue = false;
                    }

                    if (!this.InternalVerificationViaLinqQuery(mixedOrderJsonObj, name, dict[name]))
                    {
                        retValue = false;
                    }

                    if (!this.CrossJsonValueVerificationOnNameViaLinqQuery(myJsonObj, mixedOrderJsonObj, name))
                    {
                        retValue = false;
                    }

                    if (!this.CrossJsonValueVerificationOnIndexViaLinqQuery(myJsonObj, mixedOrderJsonObj, dict[name]))
                    {
                        retValue = false;
                    }
                }

                if (!retValue)
                {
                    Assert.Fail("The JsonValue did not verify as expected!");
                }
            }
            else
            {
                Assert.Fail("Failed to create the pair of JsonValues!");
            }
        }

        [TestMethod]
        public void LinqToDynamicJsonArrayTest()
        {
            JsonValue people = this.CreateArrayOfPeople();

            var match = from person in people select person;
            Assert.IsTrue(match.Count() == people.Count, "IEnumerable returned different number of elements that JsonArray contains");

            int sum = 0;
            foreach (KeyValuePair<string, JsonValue> kv in match)
            {
                sum += Int32.Parse(kv.Key);
            }

            Assert.IsTrue(sum == (people.Count * (people.Count - 1) / 2), "Not all elements of the array were enumerated exactly once");

            match = from person in people
                    where person.Value.AsDynamic().Name.ReadAs<string>().StartsWith("S")
                        && person.Value.AsDynamic().Age.ReadAs<int>() > 20
                    select person;
            Assert.IsTrue(match.Count() == 2, "Number of matches was expected to be 2 but was " + match.Count());
        }

        [TestMethod]
        public void LinqToJsonObjectTest()
        {
            JsonValue person = this.CreateArrayOfPeople()[0];
            var match = from nameValue in person select nameValue;
            Assert.IsTrue(match.Count() == 3, "IEnumerable of JsonObject returned a different number of elements than there are name value pairs in the JsonObject" + match.Count());

            List<string> missingNames = new List<string>(new string[] { "Name", "Age", "Birthday" });
            foreach (KeyValuePair<string, JsonValue> kv in match)
            {
                Assert.AreEqual(person[kv.Key], kv.Value);
                missingNames.Remove(kv.Key);
            }

            Assert.IsTrue(missingNames.Count == 0, "Not all JsonObject properties were present in the enumeration");
        }

        [TestMethod]
        public void LinqToJsonObjectAsAssociativeArrayTest()
        {
            JsonValue gameScores = new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "tomek", 12 },
                    { "suwat", 27 },
                    { "carlos", 127 },
                    { "miguel", 57 },
                    { "henrik", 2 },
                    { "joe", 15 }
                });

            var match = from score in gameScores
                        where score.Key.Contains("o") && score.Value.ReadAs<int>() > 100
                        select score;
            Assert.IsTrue(match.Count() == 1, "Incorrect number of matching game scores");
        }

        [TestMethod]
        public void LinqToJsonPrimitiveTest()
        {
            JsonValue primitive = 12;

            var match = from m in primitive select m;
            KeyValuePair<string, JsonValue>[] kv = match.ToArray();
            Assert.IsTrue(kv.Length == 1);            
            Assert.AreEqual(kv[0].Key, string.Empty);
            Assert.AreEqual(kv[0].Value, primitive);
        }

        [TestMethod]
        public void LinqToJsonUndefinedTest()
        {
            JsonValue primitive = 12;

            var match = from m in primitive.ValueOrDefault("idontexist")
                        select m;
            Assert.IsTrue(match.Count() == 0);
        }

        [TestMethod]
        public void LinqToDynamicJsonUndefinedWithFallbackTest()
        {
            JsonValue people = this.CreateArrayOfPeople();

            var match = from person in people
                        where person.Value.AsDynamic().IDontExist.IAlsoDontExist.ReadAs<int>(5) > 2
                        select person;
            Assert.IsTrue(match.Count() == people.Count, "Number of matches was expected to be " + people.Count + " but was " + match.Count());

            match = from person in people
                    where person.Value.AsDynamic().Age.ReadAs<int>(1) < 21
                    select person;
            Assert.IsTrue(match.Count() == 3);        
        }

        private JsonArray CreateArrayOfPeople()
        {
            int seed = 1;
            Random rndGen = new Random(seed);
            return new JsonArray(new List<JsonValue>()
            { 
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Alex" },
                    { "Age", 18 },
                    { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Joe" },
                    { "Age", 19 },
                    { "Birthday", DateTime.MinValue }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Chris" },
                    { "Age", 20 },
                    { "Birthday", DateTime.Now }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Jeff" },
                    { "Age", 21 },
                    { "Birthday", DateTime.MaxValue }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Carlos" },
                    { "Age", 22 },
                    { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Mohammad" },
                    { "Age", 23 },
                    { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Sara" },
                    { "Age", 24 },
                    { "Birthday", new DateTime(1998, 3, 20) }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Tomasz" },
                    { "Age", 25 },
                    { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Suwat" },
                    { "Age", 26 },
                    { "Birthday", new DateTime(1500, 12, 20) }
                }),
                new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", "Eugene" },
                    { "Age", 27 },
                    { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) }
                })
            });
        }

        private bool InternalVerificationViaLinqQuery(JsonArray sourceJson, string name, int index)
        {
            var itemsByName = from JsonValue itemByName in sourceJson
                              where (itemByName != null && (string)itemByName["Name"] == name)
                              select itemByName;
            int countByName = 0;
            foreach (JsonValue a in itemsByName)
            {
                countByName++;
            }

            Log.WriteLine("Collection contains: " + countByName + " item By Name " + name);

            var itemsByIndex = from JsonValue itemByIndex in sourceJson
                               where (itemByIndex != null && (int)itemByIndex["Index"] == index)
                               select itemByIndex;
            int countByIndex = 0;
            foreach (JsonValue a in itemsByIndex)
            {
                countByIndex++;
            }

            Log.WriteLine("Collection contains: " + countByIndex + " item By Index " + index);

            if (countByIndex != countByName)
            {
                Log.WriteLine("Count by Name = " + countByName + "; Count by Index = " + countByIndex);
                Log.WriteLine("The number of items matching the provided Name does NOT equal to that matching the provided Index, The two JsonValues are not equal!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CrossJsonValueVerificationOnNameViaLinqQuery(JsonArray sourceJson, JsonArray newJson, string name)
        {
            var itemsByName = from JsonValue itemByName in sourceJson
                              where (itemByName != null && (string)itemByName["Name"] == name)
                              select itemByName;
            int countByName = 0;
            foreach (JsonValue a in itemsByName)
            {
                countByName++;
            }

            Log.WriteLine("Original Collection contains: " + countByName + " item By Name " + name);

            var newItemsByName = from JsonValue newItemByName in newJson
                                 where (newItemByName != null && (string)newItemByName["Name"] == name)
                                 select newItemByName;
            int newcountByName = 0;
            foreach (JsonValue a in newItemsByName)
            {
                newcountByName++;
            }

            Log.WriteLine("New Collection contains: " + newcountByName + " item By Name " + name);
            
            if (countByName != newcountByName)
            {
                Log.WriteLine("Count by Original JsonValue = " + countByName + "; Count by New JsonValue = " + newcountByName);
                Log.WriteLine("The number of items matching the provided Name does NOT equal between these two JsonValues!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CrossJsonValueVerificationOnIndexViaLinqQuery(JsonArray sourceJson, JsonArray newJson, int index)
        {
            var itemsByIndex = from JsonValue itemByIndex in sourceJson
                               where (itemByIndex != null && (int)itemByIndex["Index"] == index)
                               select itemByIndex;
            int countByIndex = 0;
            foreach (JsonValue a in itemsByIndex)
            {
                countByIndex++;
            }

            Log.WriteLine("Original Collection contains: " + countByIndex + " item By Index " + index);

            var newItemsByIndex = from JsonValue newItemByIndex in newJson
                                  where (newItemByIndex != null && (int)newItemByIndex["Index"] == index)
                                  select newItemByIndex;
            int newcountByIndex = 0;
            foreach (JsonValue a in newItemsByIndex)
            {
                newcountByIndex++;
            }

            Log.WriteLine("New Collection contains: " + newcountByIndex + " item By Index " + index);
            
            if (countByIndex != newcountByIndex)
            {
                Log.WriteLine("Count by Original JsonValue = " + countByIndex + "; Count by New JsonValue = " + newcountByIndex);
                Log.WriteLine("The number of items matching the provided Index does NOT equal between these two JsonValues!");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
