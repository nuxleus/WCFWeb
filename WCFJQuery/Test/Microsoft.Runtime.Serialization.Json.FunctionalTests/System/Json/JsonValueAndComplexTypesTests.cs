namespace System.Json.Test
{
    using System;
    using System.IO;
    using System.Json;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Microsoft.ServiceModel.Web.Test.Common;
    using Microsoft.Silverlight.Cdf.Test.Common.Utility;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonValueAndComplexTypesTests
    {
        static readonly Type[] testTypes = new Type[]
            {
                typeof(DCType_1),
                typeof(StructGuid),
                typeof(StructInt16),
                typeof(DCType_3),
                typeof(SerType_4),
                typeof(SerType_5),
                typeof(DCType_7),
                typeof(DCType_9),
                typeof(SerType_11),
                typeof(DCType_15),
                typeof(DCType_16),
                typeof(DCType_18),
                typeof(DCType_19),
                typeof(DCType_20),
                typeof(SerType_22),
                typeof(DCType_25),
                typeof(SerType_26),
                typeof(DCType_31),
                typeof(DCType_32),
                typeof(SerType_33),
                typeof(DCType_34),
                typeof(DCType_36),
                typeof(DCType_38),
                typeof(DCType_40),
                typeof(DCType_42),
                typeof(DCType_65),
                typeof(ListType_1),
                typeof(ListType_2),
                typeof(BaseType),
                typeof(PolymorphicMember),
                typeof(PolymorphicAsInterfaceMember),
                typeof(CollectionsWithPolymorphicMember),
            };

        [TestMethod]
        public void CreateFromTests()
        {
            InstanceCreatorSurrogate oldSurrogate = CreatorSettings.CreatorSurrogate;
            try
            {
                CreatorSettings.CreatorSurrogate = new NoInfinityFloatSurrogate();
                DateTime now = DateTime.Now;
                int seed = (10000 * now.Year) + (100 * now.Month) + now.Day;
                Console.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);
                foreach (Type testType in testTypes)
                {
                    object instance = InstanceCreator.CreateInstanceOf(testType, rndGen);
                    JsonValue jv = JsonValue.CreateFrom(instance);

                    if (instance == null)
                    {
                        Assert.IsNull(jv);
                    }
                    else
                    {
                        DataContractJsonSerializer dcjs = new DataContractJsonSerializer(instance == null ? testType : instance.GetType());
                        string fromDCJS;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            dcjs.WriteObject(ms, instance);
                            fromDCJS = Encoding.UTF8.GetString(ms.ToArray());
                        }

                        Console.WriteLine("{0}: {1}", testType.Name, fromDCJS);

                        if (instance == null)
                        {
                            Assert.IsNull(jv);
                        }
                        else
                        {
                            string fromJsonValue = jv.ToString();
                            Assert.AreEqual(fromDCJS, fromJsonValue);
                        }
                    }
                }
            }
            finally
            {
                CreatorSettings.CreatorSurrogate = oldSurrogate;
            }
        }

        [TestMethod]
        public void ReadAsTests()
        {
            InstanceCreatorSurrogate oldSurrogate = CreatorSettings.CreatorSurrogate;
            try
            {
                CreatorSettings.CreatorSurrogate = new NoInfinityFloatSurrogate();
                DateTime now = DateTime.Now;
                int seed = (10000 * now.Year) + (100 * now.Month) + now.Day;
                Console.WriteLine("Seed: {0}", seed);
                Random rndGen = new Random(seed);

                this.ReadAsTest<DCType_1>(rndGen);
                this.ReadAsTest<StructGuid>(rndGen);
                this.ReadAsTest<StructInt16>(rndGen);
                this.ReadAsTest<DCType_3>(rndGen);
                this.ReadAsTest<SerType_4>(rndGen);
                this.ReadAsTest<SerType_5>(rndGen);
                this.ReadAsTest<DCType_7>(rndGen);
                this.ReadAsTest<DCType_9>(rndGen);
                this.ReadAsTest<SerType_11>(rndGen);
                this.ReadAsTest<DCType_15>(rndGen);
                this.ReadAsTest<DCType_16>(rndGen);
                this.ReadAsTest<DCType_18>(rndGen);
                this.ReadAsTest<DCType_19>(rndGen);
                this.ReadAsTest<DCType_20>(rndGen);
                this.ReadAsTest<SerType_22>(rndGen);
                this.ReadAsTest<DCType_25>(rndGen);
                this.ReadAsTest<SerType_26>(rndGen);
                this.ReadAsTest<DCType_31>(rndGen);
                this.ReadAsTest<DCType_32>(rndGen);
                this.ReadAsTest<SerType_33>(rndGen);
                this.ReadAsTest<DCType_34>(rndGen);
                this.ReadAsTest<DCType_36>(rndGen);
                this.ReadAsTest<DCType_38>(rndGen);
                this.ReadAsTest<DCType_40>(rndGen);
                this.ReadAsTest<DCType_42>(rndGen);
                this.ReadAsTest<DCType_65>(rndGen);
                this.ReadAsTest<ListType_1>(rndGen);
                this.ReadAsTest<ListType_2>(rndGen);
                this.ReadAsTest<BaseType>(rndGen);
                this.ReadAsTest<PolymorphicMember>(rndGen);
                this.ReadAsTest<PolymorphicAsInterfaceMember>(rndGen);
                this.ReadAsTest<CollectionsWithPolymorphicMember>(rndGen);
            }
            finally
            {
                CreatorSettings.CreatorSurrogate = oldSurrogate;
            }
        }

        void ReadAsTest<T>(Random rndGen)
        {
            T instance = InstanceCreator.CreateInstanceOf<T>(rndGen);
            DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(T));
            JsonValue jv;
            using (MemoryStream ms = new MemoryStream())
            {
                dcjs.WriteObject(ms, instance);
                Console.WriteLine("{0}: {1}", typeof(T).Name, Encoding.UTF8.GetString(ms.ToArray()));
                ms.Position = 0;
                jv = JsonValue.Load(ms);
            }

            if (instance == null)
            {
                Assert.IsNull(jv);
            }
            else
            {
                T newInstance = jv.ReadAs<T>();
                Assert.AreEqual(instance, newInstance);
            }
        }

        // Currently there are some differences in treatment of infinity between
        // JsonValue (which writes them as Infinity/-Infinity) and DataContractJsonSerializer
        // (which writes them as INF/-INF). This prevents those values from being used in the test.
        // This also allows the creation of an instance of an IEmptyInterface type, used in the test.
        class NoInfinityFloatSurrogate : InstanceCreatorSurrogate
        {
            public override bool CanCreateInstanceOf(Type type)
            {
                return type == typeof(float) || type == typeof(double) || type == typeof(IEmptyInterface);
            }

            public override object CreateInstanceOf(Type type, Random rndGen)
            {
                if (type == typeof(float))
                {
                    float result;
                    do
                    {
                        result = PrimitiveCreator.CreateInstanceOfSingle(rndGen);
                    }
                    while (float.IsInfinity(result));
                    return result;
                }
                else if (type == typeof(double))
                {
                    double result;
                    do
                    {
                        result = PrimitiveCreator.CreateInstanceOfDouble(rndGen);
                    }
                    while (double.IsInfinity(result));
                    return result;
                }
                else
                {
                    return new DerivedType(rndGen);
                }
            }
        }
    }
}
