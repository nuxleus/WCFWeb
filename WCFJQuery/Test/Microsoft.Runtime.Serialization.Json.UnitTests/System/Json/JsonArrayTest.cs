namespace Microsoft.ServiceModel.Web.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonArrayTest
    {
        [TestMethod]
        public void JsonArrayConstructorParamsTest()
        {
            JsonArray target;
            
            target = new JsonArray();
            Assert.AreEqual(0, target.Count);

            target = new JsonArray(null);
            Assert.AreEqual(0, target.Count);

            List<JsonValue> items = new List<JsonValue> { AnyInstance.AnyJsonValue1, AnyInstance.AnyJsonValue2 };
            target = new JsonArray(items.ToArray());
            ValidateJsonArrayItems(target, items);

            target = new JsonArray(items[0], items[1]);
            ValidateJsonArrayItems(target, items);

            // Invalide tests
            items.Add(AnyInstance.DefaultJsonValue);
            ExceptionTestHelper.ExpectException<ArgumentException>(() => new JsonArray(items.ToArray()));
            ExceptionTestHelper.ExpectException<ArgumentException>(() => new JsonArray(items[0], items[1], items[2]));
        }

        [TestMethod]
        public void JsonArrayConstructorEnumTest()
        {
            List<JsonValue> items = new List<JsonValue> { AnyInstance.AnyJsonValue1, AnyInstance.AnyJsonValue2, AnyInstance.AnyJsonValue3 };
            JsonArray target;
            
            target = new JsonArray(items);
            ValidateJsonArrayItems(target, items);

            ExceptionTestHelper.ExpectException<ArgumentNullException>(() => new JsonArray((IEnumerable<JsonValue>)null));
            
            items.Add(AnyInstance.DefaultJsonValue);
            ExceptionTestHelper.ExpectException<ArgumentException>(() => new JsonArray(items));
        }

        [TestMethod]
        public void AddTest()
        {
            JsonArray target = new JsonArray();
            JsonValue item = AnyInstance.AnyJsonValue1;
            Assert.IsFalse(target.Contains(item));
            target.Add(item);
            Assert.AreEqual(1, target.Count);
            Assert.AreEqual(item, target[0]);
            Assert.IsTrue(target.Contains(item));

            ExceptionTestHelper.ExpectException<ArgumentException>(() => target.Add(AnyInstance.DefaultJsonValue));
        }

        [TestMethod]
        public void AddRangeEnumTest()
        {
            List<JsonValue> items = new List<JsonValue> { AnyInstance.AnyJsonValue1, AnyInstance.AnyJsonValue2 };

            JsonArray target = new JsonArray();
            target.AddRange(items);
            ValidateJsonArrayItems(target, items);

            ExceptionTestHelper.ExpectException<ArgumentNullException>(() => new JsonArray().AddRange((IEnumerable<JsonValue>)null));

            items.Add(AnyInstance.DefaultJsonValue);
            ExceptionTestHelper.ExpectException<ArgumentException>(() => new JsonArray().AddRange(items));
        }

        [TestMethod]
        public void AddRangeParamsTest()
        {
            List<JsonValue> items = new List<JsonValue> { AnyInstance.AnyJsonValue1, AnyInstance.AnyJsonValue2, AnyInstance.AnyJsonValue3 };
            JsonArray target;

            target = new JsonArray();
            target.AddRange(items[0], items[1], items[2]);
            ValidateJsonArrayItems(target, items);

            target = new JsonArray();
            target.AddRange(items.ToArray());
            ValidateJsonArrayItems(target, items);

            target.AddRange();
            ValidateJsonArrayItems(target, items);

            items.Add(AnyInstance.DefaultJsonValue);
            ExceptionTestHelper.ExpectException<ArgumentException>(() => new JsonArray().AddRange(items[items.Count - 1]));
            ExceptionTestHelper.ExpectException<ArgumentException>(() => new JsonArray().AddRange(items));
        }

        [TestMethod]
        public void ClearTest()
        {
            JsonArray target = new JsonArray(AnyInstance.AnyJsonValue1, AnyInstance.AnyJsonValue2);
            Assert.AreEqual(2, target.Count);
            target.Clear();
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void ContainsTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;
            JsonArray target = new JsonArray(item1);
            Assert.IsTrue(target.Contains(item1));
            Assert.IsFalse(target.Contains(item2));

            target.Add(item2);
            Assert.IsTrue(target.Contains(item1));
            Assert.IsTrue(target.Contains(item2));

            target.Remove(item1);
            Assert.IsFalse(target.Contains(item1));
            Assert.IsTrue(target.Contains(item2));
        }

        [TestMethod]
        public void ReadAsComplexTypeTest()
        {
            JsonArray target = new JsonArray(AnyInstance.AnyInt, AnyInstance.AnyInt + 1, AnyInstance.AnyInt + 2);
            int[] intArray = target.ReadAs<int[]>();
            Assert.AreEqual(((JsonArray)target).Count, intArray.Length);
            for (int i = 0; i < intArray.Length; i++)
            {
                Assert.AreEqual(AnyInstance.AnyInt + i, intArray[i]);
            }
        }

        [TestMethod]
        public void CopyToTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;
            JsonArray target = new JsonArray(item1, item2);
            JsonValue[] array = new JsonValue[target.Count + 1];

            target.CopyTo(array, 0);
            Assert.AreEqual(item1, array[0]);
            Assert.AreEqual(item2, array[1]);

            target.CopyTo(array, 1);
            Assert.AreEqual(item1, array[1]);
            Assert.AreEqual(item2, array[2]);

            ExceptionTestHelper.ExpectException<ArgumentNullException>(() => target.CopyTo(null, 0));
            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(() => target.CopyTo(array, -1));
            ExceptionTestHelper.ExpectException<ArgumentException>(() => target.CopyTo(array, array.Length - target.Count + 1));
        }

        [TestMethod]
        public void IndexOfTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;
            JsonValue item3 = AnyInstance.AnyJsonValue3;
            JsonArray target = new JsonArray(item1, item2);

            Assert.AreEqual(0, target.IndexOf(item1));
            Assert.AreEqual(1, target.IndexOf(item2));
            Assert.AreEqual(-1, target.IndexOf(item3));
        }

        [TestMethod]
        public void InsertTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;
            JsonValue item3 = AnyInstance.AnyJsonValue3;
            JsonArray target = new JsonArray(item1);

            Assert.AreEqual(1, target.Count);
            target.Insert(0, item2);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(item2, target[0]);
            Assert.AreEqual(item1, target[1]);

            target.Insert(1, item3);
            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(item2, target[0]);
            Assert.AreEqual(item3, target[1]);
            Assert.AreEqual(item1, target[2]);

            target.Insert(target.Count, item2);
            Assert.AreEqual(4, target.Count);
            Assert.AreEqual(item2, target[0]);
            Assert.AreEqual(item3, target[1]);
            Assert.AreEqual(item1, target[2]);
            Assert.AreEqual(item2, target[3]);

            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(() => target.Insert(-1, item3));
            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(() => target.Insert(target.Count + 1, item1));
            ExceptionTestHelper.ExpectException<ArgumentException>(() => target.Insert(0, AnyInstance.DefaultJsonValue));
        }

        [TestMethod]
        public void RemoveTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;
            JsonValue item3 = AnyInstance.AnyJsonValue3;
            JsonArray target = new JsonArray(item1, item2, item3);

            Assert.IsTrue(target.Remove(item2));
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(item1, target[0]);
            Assert.AreEqual(item3, target[1]);

            Assert.IsFalse(target.Remove(item2));
            Assert.AreEqual(2, target.Count);
        }

        [TestMethod]
        public void RemoveAtTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;
            JsonValue item3 = AnyInstance.AnyJsonValue3;
            JsonArray target = new JsonArray(item1, item2, item3);

            target.RemoveAt(1);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(item1, target[0]);
            Assert.AreEqual(item3, target[1]);

            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(() => target.RemoveAt(-1));
            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(() => target.RemoveAt(target.Count));
        }

        [TestMethod]
        public void SaveTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = null;
            JsonValue item3 = AnyInstance.AnyJsonValue2;
            JsonArray target = new JsonArray(item1, item2, item3);

            string expected = string.Format(CultureInfo.InvariantCulture, "[{0},null,{1}]", item1, item3);
            Assert.AreEqual(expected, target.ToString());
        }

        [TestMethod]
        public void GetEnumeratorTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;

            IEnumerable<JsonValue> target = new JsonArray(item1, item2);
            IEnumerator<JsonValue> enumerator = target.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(item1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(item2, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void GetEnumeratorTest1()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;

            IEnumerable target = new JsonArray(item1, item2);
            IEnumerator enumerator = target.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(item1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(item2, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void CountTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;

            JsonArray target = new JsonArray();
            Assert.AreEqual(0, target.Count);
            target.Add(item1);
            Assert.AreEqual(1, target.Count);
            target.Add(item2);
            Assert.AreEqual(2, target.Count);
            target.Remove(item1);
            Assert.AreEqual(1, target.Count);
        }

        [TestMethod]
        public void IsReadOnlyTest()
        {
            JsonArray target = AnyInstance.AnyJsonArray;
            Assert.IsFalse(target.IsReadOnly);
        }

        [TestMethod]
        public void ItemTest()
        {
            JsonValue item1 = AnyInstance.AnyJsonValue1;
            JsonValue item2 = AnyInstance.AnyJsonValue2;

            JsonArray target = new JsonArray(item1);
            Assert.AreEqual(item1, target[0]);
            target[0] = item2;
            Assert.AreEqual(item2, target[0]);

            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(delegate { var i = target[-1]; });
            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(delegate { var i = target[target.Count]; });
            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(delegate { target[-1] = AnyInstance.AnyJsonValue1; });
            ExceptionTestHelper.ExpectException<ArgumentOutOfRangeException>(delegate { target[target.Count] = AnyInstance.AnyJsonValue2; });
            ExceptionTestHelper.ExpectException<ArgumentException>(delegate { target[0] = AnyInstance.DefaultJsonValue; });
        }

        [TestMethod]
        public void JsonTypeTest()
        {
            JsonArray target = AnyInstance.AnyJsonArray;
            Assert.AreEqual(JsonType.Array, target.JsonType);
        }

        static void ValidateJsonArrayItems(JsonArray jsonArray, IEnumerable<JsonValue> expectedItems)
        {
            List<JsonValue> expected = new List<JsonValue>(expectedItems);
            Assert.AreEqual(expected.Count, jsonArray.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], jsonArray[i]);
            }
        }
    }
}
