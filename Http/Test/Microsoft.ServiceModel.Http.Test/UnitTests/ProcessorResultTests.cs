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
    public class ProcessorResultTests
    {
        [TestMethod]
        [Description("ProcessorResult default ctor initializes properly")]
        public void ProcessorResult_Ctor()
        {
            ProcessorResult result = new ProcessorResult();
            Assert.AreEqual(ProcessorStatus.Ok, result.Status, "Default ctor should set status to ok");
            Assert.IsNull(result.Output, "Default ctor should set output to null");
        }

        [TestMethod]
        [Description("ProcessorResult properties are mutable and function correctly")]
        public void ProcessorResult_Properties()
        {
            ProcessorResult result = new ProcessorResult();

            result.Status = ProcessorStatus.Error;
            Assert.AreEqual(ProcessorStatus.Error, result.Status, "Could not set status");

            result.Output = new object[0];
            Assert.IsNotNull(result.Output, "Could not set output");
            Assert.AreEqual(0, result.Output.Length, "Output length unexpected");

            object[] expectedOutput = new object[] { 5, "hello" };
            result.Output = expectedOutput;
            Assert.AreEqual(2, result.Output.Length, "Output[2] length unexpected");
            Assert.AreEqual(5, result.Output[0], "Output[0] wrong");
            Assert.AreEqual("hello", result.Output[1], "Output[1] wrong");
        }

        [TestMethod]
        [Description("ProcessorResult generic value type returns default if output is empty")]
        public void ProcessorResult_Generic_ValueType_Empty_Output_Yields_Default_Value()
        {
            ProcessorResult<int> result = new ProcessorResult<int>();
            ProcessorResult baseResult = (ProcessorResult)result;

            Assert.IsNull(baseResult.Output, "Expected result to initialize to null output");

            int i = result.Output;
            Assert.AreEqual(default(int), i, "Null output should have yielded default");

            baseResult.Output = new object[0];
            i = result.Output;
            Assert.AreEqual(default(int), i, "Empty output should have yielded default");

            // Finally use a valid result to verify ProcessorResult does work after above failures
            baseResult.Output = new object[] { 7 };
            i = result.Output;
            Assert.AreEqual(7, i, "Valid output should have worked");
        }

        [TestMethod]
        [Description("ProcessorResult generic form works for value types")]
        public void ProcessorResult_Generic_ValueTypes()
        {
            AssertValueTypeProcessorResult<int>(-1, 0, 1, int.MaxValue, int.MaxValue);
            AssertValueTypeProcessorResult<uint>(0, 1, uint.MinValue, uint.MaxValue);

            AssertValueTypeProcessorResult<short>(-1, 0, 1, short.MinValue, short.MaxValue);
            AssertValueTypeProcessorResult<ushort>(0, 1, ushort.MinValue, ushort.MaxValue);

            AssertValueTypeProcessorResult<long>(-1, 0, 1, long.MinValue, long.MaxValue);
            AssertValueTypeProcessorResult<ulong>(0, 1, ulong.MinValue, ulong.MaxValue);

            AssertValueTypeProcessorResult<byte>(0, 1, byte.MinValue, byte.MaxValue);
            AssertValueTypeProcessorResult<sbyte>(-1, 0, 1, sbyte.MinValue, sbyte.MaxValue);

            AssertValueTypeProcessorResult<bool>(true, false);
            AssertValueTypeProcessorResult<char>('a', char.MinValue, char.MaxValue);
            AssertValueTypeProcessorResult<float>(-1.0f, 0.0f, 1.0f, float.MinValue, float.MaxValue);
            AssertValueTypeProcessorResult<double>(-1.0, 0.0, 1.0, double.MinValue, double.MaxValue);
            AssertValueTypeProcessorResult<decimal>(-1M, 0M, 1M, decimal.MinValue, decimal.MaxValue);

            AssertValueTypeProcessorResult<DateTime>(DateTime.Now, DateTime.UtcNow);
            AssertValueTypeProcessorResult<Guid>(Guid.NewGuid());
            AssertValueTypeProcessorResult<TimeSpan>(TimeSpan.MinValue, TimeSpan.MaxValue);
        }

        [TestMethod]
        [Description("ProcessorResult generic form works for nullable types")]
        public void ProcessorResult_Generic_NullableTypes()
        {
            AssertNullableProcessorResult<int>(-1, 0, 1, int.MaxValue, int.MaxValue);
            AssertNullableProcessorResult<uint>(0, 1, uint.MinValue, uint.MaxValue);

            AssertNullableProcessorResult<short>(-1, 0, 1, short.MinValue, short.MaxValue);
            AssertNullableProcessorResult<ushort>(0, 1, ushort.MinValue, ushort.MaxValue);

            AssertNullableProcessorResult<long>(-1, 0, 1, long.MinValue, long.MaxValue);
            AssertNullableProcessorResult<ulong>(0, 1, ulong.MinValue, ulong.MaxValue);

            AssertNullableProcessorResult<byte>(0, 1, byte.MinValue, byte.MaxValue);
            AssertNullableProcessorResult<sbyte>(-1, 0, 1, sbyte.MinValue, sbyte.MaxValue);

            AssertNullableProcessorResult<bool>(true, false);
            AssertNullableProcessorResult<char>('a', char.MinValue, char.MaxValue);
            AssertNullableProcessorResult<float>(-1.0f, 0.0f, 1.0f, float.MinValue, float.MaxValue);
            AssertNullableProcessorResult<double>(-1.0, 0.0, 1.0, double.MinValue, double.MaxValue);
            AssertNullableProcessorResult<decimal>(-1M, 0M, 1M, decimal.MinValue, decimal.MaxValue);

            AssertNullableProcessorResult<DateTime>(DateTime.Now, DateTime.UtcNow);
            AssertNullableProcessorResult<Guid>(Guid.NewGuid());
            AssertNullableProcessorResult<TimeSpan>(TimeSpan.MinValue, TimeSpan.MaxValue);
        }

        #region test helpers

        private void AssertValueTypeProcessorResult<T>(params T[] values) where T : struct
        {
            ProcessorResult<T> result = new ProcessorResult<T>();
            ProcessorResult baseResult = result as ProcessorResult;

            foreach (T value in values)
            {
                baseResult.Output = new object[] { value };
                Assert.AreEqual(value, result.Output, "Wrong value for ProcessorType>" + typeof(T).Name + "> for " + value);
            }

            // Attempting read from a null value should give default(T)
            baseResult.Output = new object[] { null };
            object valueFromNull = result.Output;
            Assert.AreEqual(default(T), valueFromNull, "Expected default(T) for ProcessorType>" + typeof(T).Name); 

            // Attempting read from an illegal value type should throw
            T unused;
            ExceptionAssert.ThrowsInvalidOperation(
                "Setting illegal value to ProcessorType>" + typeof(T).Name + "> should throw",
                () =>
                {
                    baseResult.Output = new object[] { "hello" };
                    unused = result.Output;
                }
                );
        }

        private void AssertNullableProcessorResult<T>(params T[] values) where T : struct
        {
            ProcessorResult<T?> result = new ProcessorResult<T?>();
            ProcessorResult baseResult = result as ProcessorResult;

            foreach (T value in values)
            {
                baseResult.Output = new object[] { value };
                Assert.IsTrue(result.Output.HasValue, "Expected HasValue==true for ProcessorType>" + typeof(T).Name + "> for " + value);
                Assert.AreEqual(value, result.Output.Value, "Wrong value for ProcessorType>" + typeof(T).Name + "> for " + value);
            }

            baseResult.Output = new object[] { null };
            T? nullableValue = result.Output;
            Assert.IsFalse(nullableValue.HasValue, "ProcessorType>" + typeof(T).Name + "> should set HasValue == false for null");

            T? unused;
            ExceptionAssert.ThrowsInvalidOperation(
                "Setting illegal value to ProcessorType>" + typeof(T).Name + "> should throw",
                () =>
                {
                    baseResult.Output = new object[] { "hello" };
                    unused = result.Output;
                }
                );
        }

        #endregion test helpers
    }
}
