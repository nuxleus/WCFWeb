namespace System.Json.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Json;
    using System.Text;
    using System.Threading;
    using Microsoft.Silverlight.Cdf.Test.Common.Utility;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JObjectFunctionalTest
    {
        static int iterationCount = 500;
        static int arrayLength = 10;

        static TextWriter Log
        {
            get
            {
                return Console.Out;
            }
        }

        [TestMethod]
        public void MixedJsonTypeFunctionalTest()
        {
            bool oldValue = CreatorSettings.CreateDateTimeWithSubMilliseconds;
            CreatorSettings.CreateDateTimeWithSubMilliseconds = false;
            try
            {
                int seed = 1;

                for (int i = 0; i < iterationCount; i++)
                {
                    seed++;
                    Log.WriteLine("Seed: {0}", seed);
                    Random rndGen = new Random(seed);

                    JsonArray sourceJson = new JsonArray(new List<JsonValue>()
                    {
                        PrimitiveCreator.CreateInstanceOfBoolean(rndGen),
                        PrimitiveCreator.CreateInstanceOfByte(rndGen),
                        PrimitiveCreator.CreateInstanceOfDateTime(rndGen),
                        PrimitiveCreator.CreateInstanceOfDateTimeOffset(rndGen),
                        PrimitiveCreator.CreateInstanceOfDecimal(rndGen),
                        PrimitiveCreator.CreateInstanceOfDouble(rndGen),
                        PrimitiveCreator.CreateInstanceOfInt16(rndGen),
                        PrimitiveCreator.CreateInstanceOfInt32(rndGen),
                        PrimitiveCreator.CreateInstanceOfInt64(rndGen),
                        PrimitiveCreator.CreateInstanceOfSByte(rndGen),
                        PrimitiveCreator.CreateInstanceOfSingle(rndGen),
                        PrimitiveCreator.CreateInstanceOfString(rndGen),
                        PrimitiveCreator.CreateInstanceOfUInt16(rndGen),
                        PrimitiveCreator.CreateInstanceOfUInt32(rndGen),
                        PrimitiveCreator.CreateInstanceOfUInt64(rndGen),
                        new JsonObject(new Dictionary<string, JsonValue>()
                        {
                            { "Boolean", PrimitiveCreator.CreateInstanceOfBoolean(rndGen) },
                            { "Byte", PrimitiveCreator.CreateInstanceOfByte(rndGen) },
                            { "DateTime", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) },
                            { "DateTimeOffset", PrimitiveCreator.CreateInstanceOfDateTimeOffset(rndGen) },
                            { "Decimal", PrimitiveCreator.CreateInstanceOfDecimal(rndGen) },
                            { "Double", PrimitiveCreator.CreateInstanceOfDouble(rndGen) },
                            { "Int16", PrimitiveCreator.CreateInstanceOfInt16(rndGen) },
                            { "Int32", PrimitiveCreator.CreateInstanceOfInt32(rndGen) },
                            { "Int64", PrimitiveCreator.CreateInstanceOfInt64(rndGen) },
                            { "SByte", PrimitiveCreator.CreateInstanceOfSByte(rndGen) },
                            { "Single", PrimitiveCreator.CreateInstanceOfSingle(rndGen) },
                            { "String", PrimitiveCreator.CreateInstanceOfString(rndGen) },
                            { "UInt16", PrimitiveCreator.CreateInstanceOfUInt16(rndGen) },
                            { "UInt32", PrimitiveCreator.CreateInstanceOfUInt32(rndGen) },
                            { "UInt64", PrimitiveCreator.CreateInstanceOfUInt64(rndGen) }
                        })
                    });

                    JsonArray newJson = (JsonArray)JsonValue.Parse(sourceJson.ToString());
                    if (!JsonValueVerifier.Compare(sourceJson, newJson, Log))
                    {
                        Assert.Fail("MixedJsonTypeFunctionalTest failed!  The new JsonValue does not equal to the original one.");
                    }
                }
            }
            finally
            {
                CreatorSettings.CreateDateTimeWithSubMilliseconds = oldValue;
            }
        }

        [TestMethod]
        public void JsonArrayCopytoFunctionalTest()
        {
            int seed = 1;

            for (int i = 0; i < iterationCount / 10; i++)
            {
                seed++;
                Log.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);

                bool retValue = true;

                JsonArray sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);
                JsonValue[] destJson = new JsonValue[arrayLength];
                sourceJson.CopyTo(destJson, 0);

                for (int k = 0; k < destJson.Length; k++)
                {
                    if (destJson[k] != sourceJson[k])
                    {
                        retValue = false;
                    }
                }

                if (!retValue)
                {
                    Assert.Fail("[JsonArrayCopytoFunctionalTest] JsonArray.CopyTo() failed to function properly. destJson.GetLength(0) = " + destJson.GetLength(0));
                }
            }
        }

        [TestMethod]
        public void JsonArrayAddRemoveFunctionalTest()
        {
            int seed = 1;

            for (int i = 0; i < iterationCount / 10; i++)
            {
                seed++;
                Log.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);
                bool retValue = true;

                JsonArray sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);
                JsonValue[] cloneJson = SpecialJsonValueHelper.CreatePrePopulatedJsonValueArray(seed, 3);

                // JsonArray.AddRange(JsonValue[])
                sourceJson.AddRange(cloneJson);
                if (sourceJson.Count != arrayLength + cloneJson.Length)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.AddRange(JsonValue[]) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.AddRange(JsonValue[]) passed test.");
                }

                // JsonArray.RemoveAt(int)
                int count = sourceJson.Count;
                for (int j = 0; j < count; j++)
                {
                    sourceJson.RemoveAt(0);
                }

                if (sourceJson.Count > 0)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.RemoveAt(int) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.RemoveAt(int) passed test.");
                }

                // JsonArray.JsonType
                if (sourceJson.JsonType != JsonType.Array)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.JsonType failed to function properly.");
                    retValue = false;
                }

                // JsonArray.Clear()
                sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);
                sourceJson.Clear();
                if (sourceJson.Count > 0)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.Clear() failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.Clear() passed test.");
                }

                // JsonArray.AddRange(JsonValue)
                sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);

                // adding one additional value to the array
                sourceJson.AddRange(SpecialJsonValueHelper.GetRandomJsonPrimitives(seed));
                if (sourceJson.Count != arrayLength + 1)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.AddRange(JsonValue) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.AddRange(JsonValue) passed test.");
                }

                // JsonArray.AddRange(IEnumerable<JsonValue> items)
                sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);
                MyJsonValueCollection<JsonValue> myCols = new MyJsonValueCollection<JsonValue>();
                myCols.Add(new JsonPrimitive(PrimitiveCreator.CreateInstanceOfUInt32(rndGen)));
                myCols.Add(new JsonPrimitive(PrimitiveCreator.CreateInstanceOfString(rndGen, false)));
                myCols.Add(new JsonPrimitive(PrimitiveCreator.CreateInstanceOfDateTime(rndGen)));

                // adding 3 additional value to the array
                sourceJson.AddRange(myCols);
                if (sourceJson.Count != arrayLength + 3)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.AddRange(IEnumerable<JsonValue> items) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.AddRange(IEnumerable<JsonValue> items) passed test.");
                }

                // JsonArray[index].set_Item
                sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);
                string temp = PrimitiveCreator.CreateInstanceOfString(rndGen, false);
                sourceJson[1] = temp;
                if ((string)sourceJson[1] != temp)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray[index].set_Item failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray[index].set_Item passed test.");
                }

                // JsonArray.Remove(JsonValue)
                count = sourceJson.Count;
                for (int j = 0; j < count; j++)
                {
                    sourceJson.Remove(sourceJson[0]);
                }

                if (sourceJson.Count > 0)
                {
                    Log.WriteLine("[JsonArrayAddRemoveFunctionalTest] JsonArray.Remove(JsonValue) failed to function properly.");
                    retValue = false;
                }

                if (!retValue)
                {
                    Assert.Fail("[JsonArrayAddRemoveFunctionalTest] Test failed!");
                }
            }
        }

        [TestMethod]
        public void JsonArrayItemsFunctionalTest()
        {
            int seed = 1;

            for (int i = 0; i < iterationCount / 10; i++)
            {
                seed++;
                Log.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);
                bool retValue = true;

                // JsonArray.Contains(JsonValue)
                // JsonArray.IndexOf(JsonValue)
                JsonArray sourceJson = SpecialJsonValueHelper.CreatePrePopulatedJsonArray(seed, arrayLength);
                for (int j = 0; j < sourceJson.Count; j++)
                {
                    if (!sourceJson.Contains(sourceJson[j]))
                    {
                        Log.WriteLine("[JsonArrayItemsFunctionalTest] JsonArray.Contains(JsonValue) failed to function properly.");
                        retValue = false;
                    }
                    else
                    {
                        Log.WriteLine("[JsonArrayItemsFunctionalTest] JsonArray.Contains(JsonValue) passed test.");
                    }

                    if (sourceJson.IndexOf(sourceJson[j]) != j)
                    {
                        Log.WriteLine("[JsonArrayItemsFunctionalTest] JsonArray.IndexOf(JsonValue) failed to function properly.");
                        retValue = false;
                    }
                    else
                    {
                        Log.WriteLine("[JsonArrayItemsFunctionalTest] JsonArray.IndexOf(JsonValue) passed test.");
                    }
                }

                // JsonArray.Insert(int, JsonValue)
                JsonValue newItem = SpecialJsonValueHelper.GetRandomJsonPrimitives(seed);
                sourceJson.Insert(3, newItem);
                if (sourceJson[3] != newItem || sourceJson.Count != arrayLength + 1)
                {
                    Log.WriteLine("[JsonArrayItemsFunctionalTest] JsonArray.Insert(int, JsonValue) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonArrayItemsFunctionalTest] JsonArray.Insert(int, JsonValue) passed test.");
                }

                if (!retValue)
                {
                    Assert.Fail("[JsonArrayItemsFunctionalTest] Test failed!");
                }
            }
        }

        [TestMethod]
        public void JsonObjectCopytoFunctionalTest()
        {
            int seed = 1;

            for (int i = 0; i < iterationCount / 10; i++)
            {
                seed++;
                Log.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);

                bool retValue = true;

                JsonObject sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);
                KeyValuePair<string, JsonValue>[] destJson = new KeyValuePair<string, JsonValue>[arrayLength];
                if (sourceJson != null && destJson != null)
                {
                    sourceJson.CopyTo(destJson, 0);
                }
                else
                {
                    Log.WriteLine("[JsonObjectCopytoFunctionalTest] sourceJson.ToString() = " + sourceJson.ToString());
                    Log.WriteLine("[JsonObjectCopytoFunctionalTest] destJson.ToString() = " + destJson.ToString());
                    Assert.Fail("[JsonObjectCopytoFunctionalTest] failed to create the source JsonObject object.");
                    return;
                }

                if (destJson.Length == arrayLength)
                {
                    for (int k = 0; k < destJson.Length; k++)
                    {
                        JsonValue temp;
                        sourceJson.TryGetValue(k.ToString(), out temp);
                        if (!(temp != null && destJson[k].Value == temp))
                        {
                            retValue = false;
                        }
                    }
                }
                else
                {
                    retValue = false;
                }

                if (!retValue)
                {
                    Assert.Fail("[JsonObjectCopytoFunctionalTest] JsonObject.CopyTo() failed to function properly. destJson.GetLength(0) = " + destJson.GetLength(0));
                }
            }
        }

        [TestMethod]
        public void JsonObjectAddRemoveFunctionalTest()
        {
            int seed = 1;

            for (int i = 0; i < iterationCount / 10; i++)
            {
                seed++;
                Log.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);
                bool retValue = true;

                JsonObject sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);

                // JsonObject.JsonType
                if (sourceJson.JsonType != JsonType.Object)
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonArray.JsonType failed to function properly.");
                    retValue = false;
                }

                // JsonObject.Add(KeyValuePair<string, JsonValue> item)
                // JsonObject.Add(string key, JsonValue value)
                // + various numers below so .AddRange() won't try to add an already existing value
                sourceJson.Add(SpecialJsonValueHelper.GetUniqueNonNullInstanceOfString(seed + 3, sourceJson), SpecialJsonValueHelper.GetUniqueValue(seed, sourceJson));
                KeyValuePair<string, JsonValue> kvp;
                int startingSeed = seed + 1;
                do
                {
                    kvp = SpecialJsonValueHelper.CreatePrePopulatedKeyValuePair(startingSeed);
                    startingSeed++;
                }
                while (sourceJson.ContainsKey(kvp.Key));

                sourceJson.Add(kvp);
                do
                {
                    kvp = SpecialJsonValueHelper.CreatePrePopulatedKeyValuePair(startingSeed);
                    startingSeed++;
                }
                while (sourceJson.ContainsKey(kvp.Key));

                sourceJson.Add(kvp);
                if (sourceJson.Count != arrayLength + 3)
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.Add() failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.Add() passed test.");
                }

                // JsonObject.Clear()
                sourceJson.Clear();
                if (sourceJson.Count > 0)
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.Clear() failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.Clear() passed test.");
                }

                // JsonObject.AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items)
                sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);

                // + various numers below so .AddRange() won't try to add an already existing value
                sourceJson.AddRange(SpecialJsonValueHelper.CreatePrePopulatedListofKeyValuePair(seed + 13 + (arrayLength * 2), 5));
                if (sourceJson.Count != arrayLength + 5)
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items) passed test.");
                }

                // JsonObject.AddRange(params KeyValuePair<string, JsonValue>[] items)
                sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);

                // + various numers below so .AddRange() won't try to add an already existing value
                KeyValuePair<string, JsonValue> item1 = SpecialJsonValueHelper.CreatePrePopulatedKeyValuePair(seed + arrayLength + 41);
                KeyValuePair<string, JsonValue> item2 = SpecialJsonValueHelper.CreatePrePopulatedKeyValuePair(seed + arrayLength + 47);
                KeyValuePair<string, JsonValue> item3 = SpecialJsonValueHelper.CreatePrePopulatedKeyValuePair(seed + arrayLength + 53);
                sourceJson.AddRange(new KeyValuePair<string, JsonValue>[] { item1, item2, item3 });
                if (sourceJson.Count != arrayLength + 3)
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.AddRange(params KeyValuePair<string, JsonValue>[] items) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.AddRange(params KeyValuePair<string, JsonValue>[] items) passed test.");
                }

                sourceJson.Clear();

                // JsonObject.Remove(Key)
                sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);
                int count = sourceJson.Count;
                List<string> keys = new List<string>(sourceJson.Keys);
                foreach (string key in keys)
                {
                    sourceJson.Remove(key);
                }

                if (sourceJson.Count > 0)
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.Remove(Key) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectAddRemoveFunctionalTest] JsonObject.Remove(Key) passed test.");
                }

                if (!retValue)
                {
                    Assert.Fail("[JsonObjectAddRemoveFunctionalTest] Test failed!");
                }
            }
        }

        [TestMethod]
        public void JsonObjectItemsFunctionalTest()
        {
            int seed = 1;

            for (int i = 0; i < iterationCount / 10; i++)
            {
                seed++;
                Log.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);
                bool retValue = true;

                JsonObject sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);

                // JsonObject[key].set_Item
                sourceJson["1"] = new JsonPrimitive(true);
                if (sourceJson["1"].ToString() != "true")
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject[key].set_Item failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject[key].set_Item passed test.");
                }

                // ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item)
                KeyValuePair<string, System.Json.JsonValue> kp = new KeyValuePair<string, JsonValue>("5", sourceJson["5"]);
                if (!((ICollection<KeyValuePair<string, JsonValue>>)sourceJson).Contains(kp))
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item) passed test.");
                }

                // ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly
                if (((ICollection<KeyValuePair<string, JsonValue>>)sourceJson).IsReadOnly)
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly passed test.");
                }

                // ICollection<KeyValuePair<string, JsonValue>>.Add(KeyValuePair<string, JsonValue> item)
                kp = new KeyValuePair<string, JsonValue>("100", new JsonPrimitive(100));
                ((ICollection<KeyValuePair<string, JsonValue>>)sourceJson).Add(kp);
                if (sourceJson.Count != arrayLength + 1)
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.Add(KeyValuePair<string, JsonValue> item) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.Add(KeyValuePair<string, JsonValue> item) passed test.");
                }

                // ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item)
                ((ICollection<KeyValuePair<string, JsonValue>>)sourceJson).Remove(kp);
                if (sourceJson.Count != arrayLength)
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item) failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item) passed test.");
                }

                // ICollection<KeyValuePair<string, JsonValue>>.GetEnumerator()
                JsonObject jo = new JsonObject { { "member 1", 123 }, { "member 2", new JsonArray { 1, 2, 3 } } };
                List<string> expected = new List<string> { "member 1 - 123", "member 2 - [1,2,3]" };
                expected.Sort();
                IEnumerator<KeyValuePair<string, JsonValue>> ko = ((ICollection<KeyValuePair<string, JsonValue>>)jo).GetEnumerator();
                List<string> actual = new List<string>();
                ko.Reset();
                ko.MoveNext();
                do
                {
                    actual.Add(String.Format("{0} - {1}", ko.Current.Key, ko.Current.Value));
                    Log.WriteLine("added one item: {0}", String.Format("{0} - {1}", ko.Current.Key, ko.Current.Value));
                    ko.MoveNext();
                }
                while (ko.Current.Value != null);

                actual.Sort();
                if (!JsonValueVerifier.CompareStringLists(expected, actual))
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.GetEnumerator() failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] ICollection<KeyValuePair<string, JsonValue>>.GetEnumerator() passed test.");
                }

                // JsonObject.Values
                sourceJson = SpecialJsonValueHelper.CreateIndexPopulatedJsonObject(seed, arrayLength);
                JsonValue[] manyValues = SpecialJsonValueHelper.CreatePrePopulatedJsonValueArray(seed, arrayLength);
                JsonObject jov = new JsonObject();
                for (int j = 0; j < manyValues.Length; j++)
                {
                    jov.Add("member" + j, manyValues[j]);
                }

                List<string> expectedList = new List<string>();
                foreach (JsonValue v in manyValues)
                {
                    expectedList.Add(v.ToString());
                }

                expectedList.Sort();
                List<string> actualList = new List<string>();
                foreach (JsonValue v in jov.Values)
                {
                    actualList.Add(v.ToString());
                }

                actualList.Sort();
                if (!JsonValueVerifier.CompareStringLists(expectedList, actualList))
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject.Values failed to function properly.");
                    retValue = false;
                }
                else
                {
                    Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject.Values passed test.");
                }

                for (int j = 0; j < sourceJson.Count; j++)
                {
                    // JsonObject.Contains(Key)
                    if (!sourceJson.ContainsKey(j.ToString()))
                    {
                        Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject.Contains(Key) failed to function properly.");
                        retValue = false;
                    }
                    else
                    {
                        Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject.Contains(Key) passed test.");
                    }

                    // JsonObject.TryGetValue(String, out JsonValue)
                    JsonValue retJson;
                    if (!sourceJson.TryGetValue(j.ToString(), out retJson))
                    {
                        Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject.TryGetValue(String, out JsonValue) failed to function properly.");
                        retValue = false;
                    }
                    else if (retJson != sourceJson[j.ToString()])
                    {
                        // JsonObjectthis[string key]
                        Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject[string key] or JsonObject.TryGetValue(String, out JsonValue) failed to function properly.");
                        retValue = false;
                    }
                    else
                    {
                        Log.WriteLine("[JsonObjectItemsFunctionalTest] JsonObject.TryGetValue(String, out JsonValue) & JsonObject[string key] passed test.");
                    }
                }

                if (!retValue)
                {
                    Assert.Fail("[JsonObjectItemsFunctionalTest] Test failed!");
                }
            }
        }

        [TestMethod]
        public void GettingIntegerValueTest()
        {
            string json = "{\"byte\":160,\"sbyte\":-89,\"short\":12345,\"ushort\":65530," +
                "\"int\":1234567890,\"uint\":3000000000,\"long\":1234567890123456," +
                "\"ulong\":10000000000000000000}";
            Dictionary<string, object> expected = new Dictionary<string, object>();
            expected.Add("byte", (byte)160);
            expected.Add("sbyte", (sbyte)-89);
            expected.Add("short", (short)12345);
            expected.Add("ushort", (ushort)65530);
            expected.Add("int", (int)1234567890);
            expected.Add("uint", (uint)3000000000);
            expected.Add("long", (long)1234567890123456L);
            expected.Add("ulong", (((ulong)5000000000000000000L) * 2));
            JsonObject jo = (JsonObject)JsonValue.Parse(json);
            bool success = true;
            foreach (string key in jo.Keys)
            {
                object expectedObj = expected[key];
                Log.WriteLine("Testing for type = {0}", key);
                try
                {
                    switch (key)
                    {
                        case "byte":
                            Assert.AreEqual<byte>((byte)expectedObj, (byte)jo[key]);
                            break;
                        case "sbyte":
                            Assert.AreEqual<sbyte>((sbyte)expectedObj, (sbyte)jo[key]);
                            break;
                        case "short":
                            Assert.AreEqual<short>((short)expectedObj, (short)jo[key]);
                            break;
                        case "ushort":
                            Assert.AreEqual<ushort>((ushort)expectedObj, (ushort)jo[key]);
                            break;
                        case "int":
                            Assert.AreEqual<int>((int)expectedObj, (int)jo[key]);
                            break;
                        case "uint":
                            Assert.AreEqual<uint>((uint)expectedObj, (uint)jo[key]);
                            break;
                        case "long":
                            Assert.AreEqual<long>((long)expectedObj, (long)jo[key]);
                            break;
                        case "ulong":
                            Assert.AreEqual<ulong>((ulong)expectedObj, (ulong)jo[key]);
                            break;
                    }
                }
                catch (InvalidCastException e)
                {
                    Log.WriteLine("Caught InvalidCastException (Bug 15713): {0}", e);
                    success = false;
                }
            }

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void GettingFloatingPointValueTest()
        {
            string json = "{\"float\":1.23,\"double\":1.23e+290,\"decimal\":1234567890.123456789}";
            Dictionary<string, object> expected = new Dictionary<string, object>();
            expected.Add("float", 1.23f);
            expected.Add("double", 1.23e+290);
            expected.Add("decimal", 1234567890.123456789m);
            JsonObject jo = (JsonObject)JsonValue.Parse(json);
            bool success = true;
            foreach (string key in jo.Keys)
            {
                object expectedObj = expected[key];
                Log.WriteLine("Testing for type = {0}", key);
                try
                {
                    switch (key)
                    {
                        case "float":
                            Assert.AreEqual<float>((float)expectedObj, (float)jo[key]);
                            break;
                        case "double":
                            Assert.AreEqual<double>((double)expectedObj, (double)jo[key]);
                            break;
                        case "decimal":
                            Assert.AreEqual<decimal>((decimal)expectedObj, (decimal)jo[key]);
                            break;
                    }
                }
                catch (InvalidCastException e)
                {
                    Log.WriteLine("Caught InvalidCastException (Bug 15713): {0}", e);
                    success = false;
                }
            }

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestInvalidOperations()
        {
            JsonArray ja = new JsonArray { 1, null, "hello" };
            JsonObject jo = new JsonObject
            {
                { "first", 1 },
                { "second", null },
                { "third", "hello" },
            };
            JsonPrimitive jp = new JsonPrimitive("hello");

            try
            {
                Assert.Fail("jp[\"hello\"] should fail: " + jp["hello"].ToString());
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                Assert.Fail("ja[\"hello\"] should fail: " + ja["hello"].ToString());
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                jp["hello"] = "This shouldn't happen";
                Assert.Fail("Expected exception not thrown for jp[\"hello\"] setter");
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                ja["hello"] = "This shouldn't happen";
                Assert.Fail("Expected exception not thrown for ja[\"hello\"] setter");
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                Assert.Fail("jp[1] should fail: " + jp[1].ToString());
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                Assert.Fail("jo[0] should fail: " + jo[1].ToString());
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                jp[0] = "This shouldn't happen";
                Assert.Fail("Expected exception not thrown for jp[0] setter");
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                jo[0] = "This shouldn't happen";
                Assert.Fail("Expected exception not thrown for jo[0] setter");
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                Assert.Fail("(DateTimeOffset)jp[\"hello\"] should fail: " + (DateTimeOffset)jp);
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                Assert.Fail("(Char)jp[\"hello\"] should fail: " + (char)jp);
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                JsonPrimitive jprim = new JsonPrimitive(false);
                Assert.Fail("(System.Int16)jprim should fail: " + (short)jprim);
            }
            catch (InvalidCastException)
            {
            }

            try
            {
                JsonObject jotemp = new JsonObject(null);
            }
            catch (ArgumentNullException)
            {
            }
        }
        
        [TestMethod]
        public void TestDeeplyNestedObjectGraph()
        {
            JsonObject jo = new JsonObject();
            JsonObject current = jo;
            StringBuilder builderExpected = new StringBuilder();
            builderExpected.Append('{');
            int depth = 10000;
            for (int i = 0; i < depth; i++)
            {
                JsonObject next = new JsonObject();
                string key = i.ToString(CultureInfo.InvariantCulture);
                builderExpected.AppendFormat("\"{0}\":{{", key);
                current.Add(key, next);
                current = next;
            }

            for (int i = 0; i < depth + 1; i++)
            {
                builderExpected.Append('}');
            }

            Assert.AreEqual(builderExpected.ToString(), jo.ToString());
        }

        [TestMethod]
        public void TestDeeplyNestedArrayGraph()
        {
            JsonArray ja = new JsonArray();
            JsonArray current = ja;
            StringBuilder builderExpected = new StringBuilder();
            builderExpected.Append('[');
            int depth = 10000;
            for (int i = 0; i < depth; i++)
            {
                JsonArray next = new JsonArray();
                builderExpected.Append('[');
                current.Add(next);
                current = next;
            }

            for (int i = 0; i < depth + 1; i++)
            {
                builderExpected.Append(']');
            }

            Assert.AreEqual(builderExpected.ToString(), ja.ToString());
        }

        [TestMethod]
        public void TestDeeplyNestedObjectAndArrayGraph()
        {
            JsonObject jo = new JsonObject();
            JsonObject current = jo;
            StringBuilder builderExpected = new StringBuilder();
            builderExpected.Append('{');
            int depth = 10000;
            for (int i = 0; i < depth; i++)
            {
                JsonObject next = new JsonObject();
                string key = i.ToString(CultureInfo.InvariantCulture);
                builderExpected.AppendFormat("\"{0}\":[{{", key);
                current.Add(key, new JsonArray(next));
                current = next;
            }

            for (int i = 0; i < depth; i++)
            {
                builderExpected.Append("}]");
            }

            builderExpected.Append('}');

            Assert.AreEqual(builderExpected.ToString(), jo.ToString());
        }

        [TestMethod]
        public void TestConcurrentToString()
        {
            bool exceptionThrown = false;
            bool incorrectValue = false;
            JsonObject jo = new JsonObject();
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            for (int i = 0; i < 100000; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                string key = i.ToString(CultureInfo.InvariantCulture);
                jo.Add(key, i);
                sb.AppendFormat("\"{0}\":{0}", key);
            }

            sb.Append('}');
            string expected = sb.ToString();

            int numberOfThreads = 5;
            Thread[] threads = new Thread[numberOfThreads];
            for (int i = 0; i < numberOfThreads; i++)
            {
                threads[i] = new Thread(new ThreadStart(delegate
                {
                    for (int j = 0; j < 10; j++)
                    {
                        try
                        {
                            string str = jo.ToString();
                            if (str != expected)
                            {
                                incorrectValue = true;
                                Console.WriteLine("Value is incorrect");
                            }
                        }
                        catch (Exception e)
                        {
                            exceptionThrown = true;
                            Console.WriteLine("Exception thrown: {0}", e);
                        }
                    }
                }));
            }

            for (int i = 0; i < numberOfThreads; i++)
            {
                threads[i].Start();
            }

            for (int i = 0; i < numberOfThreads; i++)
            {
                threads[i].Join();
            }

            Assert.IsFalse(incorrectValue);
            Assert.IsFalse(exceptionThrown);
        }

        class MyJsonValueCollection<JsonValue> : System.Collections.Generic.IEnumerable<JsonValue>
        {
            List<JsonValue> internalList = new List<JsonValue>();

            public MyJsonValueCollection()
            {
            }

            public void Add(JsonValue obj)
            {
                this.internalList.Add(obj);
            }

            public IEnumerator<JsonValue> GetEnumerator()
            {
                return this.internalList.GetEnumerator();
            }

            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}