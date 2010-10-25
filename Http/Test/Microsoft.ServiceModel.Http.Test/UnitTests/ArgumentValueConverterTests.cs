// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ArgumentValueConverterTests
    {
        [TestMethod]
        [Description("ArgumentValueConverter handles nullables correctly")]
        public void ArgumentValueConverter_Nullables()
        {
            AssertNullableConverts<int>(-1, 0, 1, int.MinValue, int.MaxValue);
            AssertNullableConverts<uint>(0, 1, uint.MinValue, uint.MaxValue);

            AssertNullableConverts<short>(-1, 0, 1, short.MinValue, short.MaxValue);
            AssertNullableConverts<ushort>(0, 1, ushort.MinValue, ushort.MaxValue);

            AssertNullableConverts<long>(-1, 0, 1, long.MinValue, long.MaxValue);
            AssertNullableConverts<ulong>(0, 1, ulong.MinValue, ulong.MaxValue);

            AssertNullableConverts<byte>(0, 1, byte.MinValue, byte.MaxValue);
            AssertNullableConverts<sbyte>(-1, 0, 1, sbyte.MinValue, sbyte.MaxValue);

            AssertNullableConverts<bool>(true, false);
            AssertNullableConverts<char>('a', char.MinValue, char.MaxValue);
            AssertNullableConverts<float>(-1.0f, 0.0f, 1.0f, float.MinValue, float.MaxValue);
            AssertNullableConverts<double>(-1.0, 0.0, 1.0, double.MinValue, double.MaxValue);
            AssertNullableConverts<decimal>(-1M, 0M, 1M, decimal.MinValue, decimal.MaxValue);

            AssertNullableConverts<DateTime>(DateTime.Now, DateTime.UtcNow);
            AssertNullableConverts<Guid>(Guid.NewGuid());
            AssertNullableConverts<TimeSpan>(TimeSpan.MinValue, TimeSpan.MaxValue);
            AssertNullableConverts<CustomValueType>(new CustomValueType("hello"));
        }

        [TestMethod]
        [Description("ArgumentValueConverter handles value types correctly")]
        public void ArgumentValueConverter_ValueTypes()
        {
            AssertValueTypeConverts<int>(-1, 0, 1, int.MinValue, int.MaxValue);
            AssertValueTypeConverts<uint>(0, 1, uint.MinValue, uint.MaxValue);

            AssertValueTypeConverts<short>(-1, 0, 1, short.MinValue, short.MaxValue);
            AssertValueTypeConverts<ushort>(0, 1, ushort.MinValue, ushort.MaxValue);

            AssertValueTypeConverts<long>(-1, 0, 1, long.MinValue, long.MaxValue);
            AssertValueTypeConverts<ulong>(0, 1, ulong.MinValue, ulong.MaxValue);

            AssertValueTypeConverts<byte>(0, 1, byte.MinValue, byte.MaxValue);
            AssertValueTypeConverts<sbyte>(-1, 0, 1, sbyte.MinValue, sbyte.MaxValue);

            AssertValueTypeConverts<bool>(true, false);
            AssertValueTypeConverts<char>('a', char.MinValue, char.MaxValue);
            AssertValueTypeConverts<float>(-1.0f, 0.0f, 1.0f, float.MinValue, float.MaxValue);
            AssertValueTypeConverts<double>(-1.0, 0.0, 1.0, double.MinValue, double.MaxValue);
            AssertValueTypeConverts<decimal>(-1M, 0M, 1M, decimal.MinValue, decimal.MaxValue);

            AssertValueTypeConverts<DateTime>(DateTime.Now, DateTime.UtcNow);
            AssertValueTypeConverts<Guid>(Guid.NewGuid());
            AssertValueTypeConverts<TimeSpan>(TimeSpan.MinValue, TimeSpan.MaxValue);
            AssertValueTypeConverts<CustomValueType>(new CustomValueType("hello"));
        }

        [TestMethod]
        [Description("ArgumentValueConverter handles reference types correctly")]
        public void ArgumentValueConverter_ReferenceTypes()
        {
            AssertReferenceTypeConverts<string>("", String.Empty, "hello");
            AssertReferenceTypeConverts<CustomReferenceType>(new CustomReferenceType("hello"));
        }

        private void AssertNullableConverts<T>(params T[] values) where T : struct
        {
            // Common factory should detect and return nullable converter
            ArgumentValueConverter<Nullable<T>> converter = ArgumentValueConverter.CreateValueConverter<Nullable<T>>();
            //Assert.AreEqual(typeof(ArgumentValueConverter.NullableConverter<Nullable<T>>), converter.GetType(), "Did not create a nullable converter");

            // A clean null should convert
            Nullable<T> result = converter.ConvertFrom(null);
            Assert.IsFalse(result.HasValue, "Nullable<" + typeof(T).Name + "> null should convert to nullable with HasValue==false");

            // A default(T) should convert
            result = converter.ConvertFrom(default(T));
            Assert.IsTrue(result.HasValue, "default(" + typeof(T).Name + ") should convert to nullable with HasValue==true");
            Assert.AreEqual(default(T), result.Value, "default(" + typeof(T).Name + ") did not convert correctly");

            // Loop over the sample values
            foreach (T value in values)
            {
                // The underlying type should be convertable
                result = converter.ConvertFrom(value);
                Assert.IsTrue(result.HasValue, "Nullable<" + typeof(T).Name + "> value should convert to nullable with HasValue==true");
                Assert.AreEqual(value, result.Value, "Nullable<" + typeof(T).Name + "> expected value to convert correctly");

                // A nullable of the underlying type should be convertable
                Nullable<T> nullableOfT = new Nullable<T>(value);
                result = converter.ConvertFrom(nullableOfT);
                Assert.IsTrue(result.HasValue, "Nullable<" + typeof(T).Name + "> value should convert to Nullable<T> with HasValue==true");
                Assert.AreEqual(nullableOfT, result, "Nullable<" + typeof(T).Name + "> expected Nullable<T> value to convert correctly");
            }

            // Object of the wrong type should throw
            ExceptionAssert.ThrowsInvalidOperation(
                "Asking NullableConverter to convert wrong type should throw",
                () => converter.ConvertFrom(new object())
                );
        }

        private void AssertValueTypeConverts<T>(params T[] values) where T : struct
        {
            // Common factory should detect and return nullable converter
            ArgumentValueConverter<T> converter = ArgumentValueConverter.CreateValueConverter<T>();
            Assert.AreEqual(typeof(ArgumentValueConverter.ValueTypeConverter<T>), converter.GetType(), "Did not create a pass through converter");

            // Loop over the sample values
            foreach (T value in values)
            {
                T result = converter.ConvertFrom(value);
                Assert.AreEqual(value, result, "Passthrough<" + typeof(T).Name + " failed to convert");
            }

            // A null should convert to default(T)
            object valueFromNull = converter.ConvertFrom(null);
            Assert.IsNotNull(valueFromNull, "Value type conversion from null cannot be null");
            Assert.AreEqual(default(T), valueFromNull, "Value type conversion from null should be default(T)");

            ExceptionAssert.ThrowsInvalidOperation(
            "Attempting to convert a string to a value type should throw",
            () => converter.ConvertFrom("hello")
            );
        }

        private void AssertReferenceTypeConverts<T>(params T[] values)
        {
            // Common factory should detect and return nullable converter
            ArgumentValueConverter<T> converter = ArgumentValueConverter.CreateValueConverter<T>();
            Assert.AreEqual(typeof(ArgumentValueConverter.ReferenceTypeConverter<T>), converter.GetType(), "Did not create a pass through converter");

            T result;
            // Loop over the sample values
            foreach (T value in values)
            {
                result = converter.ConvertFrom(value);
                Assert.AreEqual(value, result, "Passthrough<" + typeof(T).Name + " failed to convert");
            }

            // A reference type should be able to convert null
            result = converter.ConvertFrom(null);
            Assert.IsNull(result, "Converting reference type " + typeof(T).Name + " failed to convert null");
        }

        public struct CustomValueType
        {
            private string value;
            public CustomValueType(string value)
            {
                this.value = value;
            }
        }

        public class CustomReferenceType
        {
            private string value;
            public CustomReferenceType(string value)
            {
                this.value = value;
            }
        }
    }
}
