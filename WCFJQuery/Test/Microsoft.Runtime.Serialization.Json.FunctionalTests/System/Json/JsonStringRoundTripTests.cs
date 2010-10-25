namespace System.Json.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Json;
    using Microsoft.Silverlight.Cdf.Test.Common.Utility;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonStringRoundTripTests
    {
        static TextWriter Log
        {
            get { return Console.Out; }
        }

        [TestMethod]
        public void ValidJsonObjectRoundTrip()
        {
            bool oldValue = CreatorSettings.CreateDateTimeWithSubMilliseconds;
            CreatorSettings.CreateDateTimeWithSubMilliseconds = false;
            try
            {
                int seed = 1;
                Console.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);

                JsonObject sourceJson = new JsonObject(new Dictionary<string, JsonValue>()
                {
                    { "Name", PrimitiveCreator.CreateInstanceOfString(rndGen) },
                    { "Age", PrimitiveCreator.CreateInstanceOfInt32(rndGen) },
                    { "DateTimeOffset", PrimitiveCreator.CreateInstanceOfDateTimeOffset(rndGen) },
                    { "Birthday", PrimitiveCreator.CreateInstanceOfDateTime(rndGen) }
                });
                sourceJson.Add("NewItem1", PrimitiveCreator.CreateInstanceOfString(rndGen));
                sourceJson.Add(new KeyValuePair<string, JsonValue>("NewItem2", PrimitiveCreator.CreateInstanceOfString(rndGen)));

                JsonObject newJson = (JsonObject)JsonValue.Parse(sourceJson.ToString());

                newJson.Remove("NewItem1");
                sourceJson.Remove("NewItem1");
                if (newJson.ContainsKey("NewItem1"))
                {
                    Assert.Fail("Test failed!  JsonObject.Remove failed to remove the targeted key value pair, the Key = NewItem1");
                }

                if (!JsonValueVerifier.Compare(sourceJson, newJson, Log))
                {
                    Assert.Fail("Test failed!  The new JsonValue does not equal to the original one.");
                }
            }
            finally
            {
                CreatorSettings.CreateDateTimeWithSubMilliseconds = oldValue;
            }
        }

        [TestMethod]
        public void SimpleDateTimeTest()
        {
            JsonValue jv = DateTime.Now;
            JsonValue jv2 = JsonValue.Parse(jv.ToString());
            Assert.AreEqual(jv.ToString(), jv2.ToString());
        }

        [TestMethod]
        public void ValidJsonObjectDateTimeOffsetRoundTrip()
        {
            int seed = 1;
            Console.WriteLine("Seed: {0}", seed);
            Random rndGen = new Random(seed);

            JsonPrimitive sourceJson = new JsonPrimitive(PrimitiveCreator.CreateInstanceOfDateTimeOffset(rndGen));
            JsonPrimitive newJson = (JsonPrimitive)JsonValue.Parse(sourceJson.ToString());

            if (!JsonValueVerifier.Compare(sourceJson, newJson, Log))
            {
                Assert.Fail("Test failed!  The new JsonObject DateTimeOffset value does not equal to the original one.");
            }
        }

        [TestMethod]
        public void ValidJsonArrayRoundTrip()
        {
            bool oldValue = CreatorSettings.CreateDateTimeWithSubMilliseconds;
            CreatorSettings.CreateDateTimeWithSubMilliseconds = false;
            try
            {
                int seed = 1;
                Console.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);

                JsonArray sourceJson = new JsonArray(new JsonValue[]
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
                    PrimitiveCreator.CreateInstanceOfUInt64(rndGen)
                });

                JsonArray newJson = (JsonArray)JsonValue.Parse(sourceJson.ToString());

                Console.WriteLine("Original JsonArray object is: {0}", sourceJson);
                Console.WriteLine("Round-tripped JsonArray object is: {0}", newJson);

                if (!JsonValueVerifier.Compare(sourceJson, newJson, Log))
                {
                    Assert.Fail("Test failed!  The new JsonValue does not equal to the original one.");
                }
            }
            finally
            {
                CreatorSettings.CreateDateTimeWithSubMilliseconds = oldValue;
            }
        }
        
        [TestMethod]
        public void ValidPrimitiveStringRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("String"));
        }

        [TestMethod]
        public void ValidPrimitiveDateTimeRoundTrip()
        {
            bool oldValue = CreatorSettings.CreateDateTimeWithSubMilliseconds;
            CreatorSettings.CreateDateTimeWithSubMilliseconds = false;
            try
            {
                Assert.IsTrue(this.TestPrimitiveType("DateTime"));
            }
            finally
            {
                CreatorSettings.CreateDateTimeWithSubMilliseconds = oldValue;
            }
        }

        [TestMethod]
        public void ValidPrimitiveBooleanRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Boolean"));
        }

        [TestMethod]
        public void ValidPrimitiveByteRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Byte"));
        }

        [TestMethod]
        public void ValidPrimitiveDecimalRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Decimal"));
        }

        [TestMethod]
        public void ValidPrimitiveDoubleRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Double"));
        }

        [TestMethod]
        public void ValidPrimitiveInt16RoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Int16"));
        }

        [TestMethod]
        public void ValidPrimitiveInt32RoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Int32"));
        }

        [TestMethod]
        public void ValidPrimitiveInt64RoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Int64"));
        }

        [TestMethod]
        public void ValidPrimitiveSByteRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("SByte"));
        }

        [TestMethod]
        public void ValidPrimitiveUInt16RoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Uint16"));
        }

        [TestMethod]
        public void ValidPrimitiveUInt32RoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("UInt32"));
        }

        [TestMethod]
        public void ValidPrimitiveUInt64RoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("UInt64"));
        }

        [TestMethod]
        public void ValidPrimitiveCharRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Char"));
        }

        [TestMethod]
        public void ValidPrimitiveGuidRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Guid"));
        }

        [TestMethod]
        public void ValidPrimitiveUriRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Uri"));
        }

        [TestMethod]
        public void ValidPrimitiveNullRoundTrip()
        {
            Assert.IsTrue(this.TestPrimitiveType("Null"));
        }

        public bool TestPrimitiveType(string typeName)
        {
            bool retValue = true;
            bool specialCase = false;

            int seed = 1;
            Console.WriteLine("Seed: {0}", seed);
            Random rndGen = new Random(seed);

            JsonPrimitive sourceJson = null;
            JsonPrimitive sourceJson2;
            object tempValue = null;
            switch (typeName.ToLower())
            {
                case "boolean":
                    tempValue = PrimitiveCreator.CreateInstanceOfBoolean(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString().ToLower());
                    sourceJson2 = new JsonPrimitive((bool)tempValue);
                    break;
                case "byte":
                    tempValue = PrimitiveCreator.CreateInstanceOfByte(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((byte)tempValue);
                    break;
                case "char":
                    sourceJson2 = new JsonPrimitive((char)PrimitiveCreator.CreateInstanceOfChar(rndGen));
                    specialCase = true;
                    break;
                case "datetime":
                    tempValue = PrimitiveCreator.CreateInstanceOfDateTime(rndGen);
                    sourceJson2 = new JsonPrimitive((DateTime)tempValue);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(sourceJson2.ToString());
                    break;
                case "decimal":
                    tempValue = PrimitiveCreator.CreateInstanceOfDecimal(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(((decimal)tempValue).ToString(NumberFormatInfo.InvariantInfo));
                    sourceJson2 = new JsonPrimitive((decimal)tempValue);
                    break;
                case "double":
                    double tempDouble = PrimitiveCreator.CreateInstanceOfDouble(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempDouble.ToString("R", NumberFormatInfo.InvariantInfo));
                    sourceJson2 = new JsonPrimitive(tempDouble);
                    break;
                case "guid":
                    sourceJson2 = new JsonPrimitive(PrimitiveCreator.CreateInstanceOfGuid(rndGen));
                    specialCase = true;
                    break;
                case "int16":
                    tempValue = PrimitiveCreator.CreateInstanceOfInt16(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((short)tempValue);
                    break;
                case "int32":
                    tempValue = PrimitiveCreator.CreateInstanceOfInt32(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((int)tempValue);
                    break;
                case "int64":
                    tempValue = PrimitiveCreator.CreateInstanceOfInt64(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((long)tempValue);
                    break;
                case "sbyte":
                    tempValue = PrimitiveCreator.CreateInstanceOfSByte(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((sbyte)tempValue);
                    break;
                case "single":
                    float fltValue = PrimitiveCreator.CreateInstanceOfSingle(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(fltValue.ToString("R", NumberFormatInfo.InvariantInfo));
                    sourceJson2 = new JsonPrimitive(fltValue);
                    break;
                case "string":
                    tempValue = PrimitiveCreator.CreateInstanceOfString(rndGen, false);
                    sourceJson2 = new JsonPrimitive((string)tempValue);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(sourceJson2.ToString());
                    break;
                case "uint16":
                    tempValue = PrimitiveCreator.CreateInstanceOfUInt16(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((ushort)tempValue);
                    break;
                case "uint32":
                    tempValue = PrimitiveCreator.CreateInstanceOfUInt32(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((uint)tempValue);
                    break;
                case "uint64":
                    tempValue = PrimitiveCreator.CreateInstanceOfUInt64(rndGen);
                    sourceJson = (JsonPrimitive)JsonValue.Parse(tempValue.ToString());
                    sourceJson2 = new JsonPrimitive((ulong)tempValue);
                    break;
                case "uri":
                    sourceJson2 = new JsonPrimitive(PrimitiveCreator.CreateInstanceOfUri(rndGen));
                    specialCase = true;
                    break;
                case "null":
                    sourceJson = (JsonPrimitive)JsonValue.Parse("null");
                    sourceJson2 = null;
                    break;
                default:
                    sourceJson = null;
                    sourceJson2 = null;
                    break;
            }

            if (!specialCase)
            {
                // comparison between two constructors
                if (!JsonValueVerifier.Compare(sourceJson, sourceJson2, Log))
                {
                    Console.WriteLine("(JsonPrimitive)JsonValue.Parse(string) failed to match the results from default JsonPrimitive(obj)constructor for type {0}", typeName);
                    retValue = false;
                }

                if (sourceJson != null)
                {
                    // test JsonValue.Load(TextReader)
                    JsonPrimitive newJson = null;
                    using (StringReader sr = new StringReader(sourceJson.ToString()))
                    {
                        newJson = (JsonPrimitive)JsonValue.Load(sr);
                    }

                    if (!JsonValueVerifier.Compare(sourceJson, newJson, Log))
                    {
                        Console.WriteLine("JsonValue.Load(TextReader) failed to function properly for type {0}", typeName);
                        retValue = false;
                    }

                    // test JsonValue.Load(Stream) is located in the JObjectFromGenoTypeLib test case

                    // test JsonValue.Parse(string)
                    newJson = null;
                    newJson = (JsonPrimitive)JsonValue.Parse(sourceJson.ToString());
                    if (!JsonValueVerifier.Compare(sourceJson, newJson, Log))
                    {
                        Console.WriteLine("JsonValue.Parse(string) failed to function properly for type {0}", typeName);
                        retValue = false;
                    }
                }
            }
            else
            {
                // test JsonValue.Load(TextReader)
                JsonPrimitive newJson2 = null;
                using (StringReader sr = new StringReader(sourceJson2.ToString()))
                {
                    newJson2 = (JsonPrimitive)JsonValue.Load(sr);
                }

                if (!JsonValueVerifier.Compare(sourceJson2, newJson2, Log))
                {
                    Console.WriteLine("JsonValue.Load(TextReader) failed to function properly for type {0}", typeName);
                    retValue = false;
                }

                // test JsonValue.Load(Stream) is located in the JObjectFromGenoTypeLib test case

                // test JsonValue.Parse(string)
                newJson2 = null;
                newJson2 = (JsonPrimitive)JsonValue.Parse(sourceJson2.ToString());
                if (!JsonValueVerifier.Compare(sourceJson2, newJson2, Log))
                {
                    Console.WriteLine("JsonValue.Parse(string) failed to function properly for type {0}", typeName);
                    retValue = false;
                }
            }

            return retValue;
        }

        [TestMethod]
        public void JsonValueImplicitCastTests()
        {
            int seed = 1;
            Console.WriteLine("Seed: {0}", seed);
            Random rndGen = new Random(seed);

            this.DoImplicitCasting(String.Empty, typeof(string));
            this.DoImplicitCasting("null", typeof(string));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfString(rndGen, false), typeof(string));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfInt16(rndGen), typeof(int));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfInt32(rndGen), typeof(int));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfInt64(rndGen), typeof(int));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfUInt16(rndGen), typeof(int));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfUInt32(rndGen), typeof(int));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfUInt64(rndGen), typeof(int));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfGuid(rndGen), typeof(Guid));
            this.DoImplicitCasting(new Uri("http://bug/test?param=hello%0a"), typeof(Uri));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfChar(rndGen), typeof(char));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfBoolean(rndGen), typeof(bool));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfDateTime(rndGen), typeof(DateTime));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfDateTimeOffset(rndGen), typeof(DateTimeOffset));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfDouble(rndGen), typeof(double));
            this.DoImplicitCasting(PrimitiveCreator.CreateInstanceOfDouble(rndGen), typeof(float));
            this.DoImplicitCasting(0.12345f, typeof(double));
            this.DoImplicitCasting(0.12345f, typeof(float));
        }
        
        public void DoImplicitCasting(JsonValue jo, Type type)
        {
            bool result = false;

                // Casting
                if (jo.JsonType == JsonType.String)
                {
                    JsonValue jstr = (string)jo;
                    if (type == typeof(DateTime))
                    {
                        Console.WriteLine("{0} Value:{1}", type.Name, ((DateTime)jstr).ToString(DateTimeFormatInfo.InvariantInfo));
                    }
                    else if (type == typeof(DateTimeOffset))
                    {
                        Console.WriteLine("{0} Value:{1}", type.Name, ((DateTimeOffset)jstr).ToString(DateTimeFormatInfo.InvariantInfo));
                    }
                    else if (type == typeof(Guid))
                    {
                        Console.WriteLine("{0} Value:{1}", type.Name, (Guid)jstr);
                    }
                    else if (type == typeof(char))
                    {
                        Console.WriteLine("{0} Value:{1}", type.Name, (char)jstr);
                    }
                    else if (type == typeof(Uri))
                    {
                        Console.WriteLine("{0} Value:{1}", type.Name, ((Uri)jstr).AbsoluteUri);
                    }
                    else
                    {
                        Console.WriteLine("{0} Value:{1}", type.Name, (string)jstr);
                    }

                    if (jo.ToString() == jstr.ToString())
                    {
                        result = true;
                    }
                }
                else if (jo.JsonType == JsonType.Object)
                {
                    JsonObject jobj = new JsonObject((JsonObject)jo);

                    if (jo.ToString() == jobj.ToString())
                    {
                        result = true;
                    }
                }
                else if (jo.JsonType == JsonType.Number)
                {
                    JsonPrimitive jprim = (JsonPrimitive)jo;
                    Console.WriteLine("{0} Value:{1}", type.Name, jprim);

                    if (jo.ToString() == jprim.ToString())
                    {
                        result = true;
                    }
                }
                else if (jo.JsonType == JsonType.Boolean)
                {
                    JsonPrimitive jprim = (JsonPrimitive)jo;
                    Console.WriteLine("{0} Value:{1}", type.Name, (bool)jprim);

                    if (jo.ToString() == jprim.ToString())
                    {
                        result = true;
                    }
                }

            if (!result)
            {
                Assert.Fail("Test Failed!");
            }
        }
    }
}
