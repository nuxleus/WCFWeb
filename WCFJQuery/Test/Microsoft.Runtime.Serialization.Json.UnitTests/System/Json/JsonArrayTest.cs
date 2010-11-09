﻿namespace Microsoft.ServiceModel.Web.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Json;
    using System.Linq;
    using System.Runtime.Serialization.Json;
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
            int[] intArray1 = (int[]) target.ReadAsComplex(typeof(int[]));
            int[] intArray2 = target.ReadAsComplex<int[]>();

            Assert.AreEqual(((JsonArray)target).Count, intArray1.Length);
            Assert.AreEqual(((JsonArray)target).Count, intArray2.Length); 
            
            for (int i = 0; i < intArray1.Length; i++)
            {
                Assert.AreEqual(AnyInstance.AnyInt + i, intArray1[i]);
                Assert.AreEqual(AnyInstance.AnyInt + i, intArray2[i]);
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
        public void ChangingEventsTest()
        {
            JsonArray ja = new JsonArray(AnyInstance.AnyInt, AnyInstance.AnyBool, null);
            TestEvents(
                ja,
                arr => arr.Add(1),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs(1, JsonValueChange.Add, 3)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs(1, JsonValueChange.Add, 3)),
                });

            TestEvents(
                ja,
                arr => arr.AddRange(AnyInstance.AnyString, AnyInstance.AnyDouble),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs(AnyInstance.AnyString, JsonValueChange.Add, 4)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs(AnyInstance.AnyDouble, JsonValueChange.Add, 5)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs(AnyInstance.AnyString, JsonValueChange.Add, 4)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs(AnyInstance.AnyDouble, JsonValueChange.Add, 5)),
                });

            TestEvents(
                ja,
                arr => arr[1] = 2,
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs(2, JsonValueChange.Replace, 1)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs(AnyInstance.AnyBool, JsonValueChange.Replace, 1)),
                });

            ja = new JsonArray { 1, 2, 3 };
            TestEvents(
                ja,
                arr => arr.Insert(1, "new value"),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs("new value", JsonValueChange.Add, 1)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs("new value", JsonValueChange.Add, 1)),
                });

            TestEvents(
                ja,
                arr => arr.RemoveAt(1),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs("new value", JsonValueChange.Remove, 1)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs("new value", JsonValueChange.Remove, 1)),
                });

            TestEvents(
                ja,
                arr => arr.Clear(),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs(null, JsonValueChange.Clear, 0)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs(null, JsonValueChange.Clear, 0)),
                });

            ja = new JsonArray(1, 2, 3);
            TestEvents(
                ja,
                arr => arr.Remove(new JsonPrimitive("Not there")),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>());

            JsonValue elementInArray = ja[1];
            TestEvents(
                ja,
                arr => arr.Remove(elementInArray),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, ja, new JsonValueChangeEventArgs(elementInArray, JsonValueChange.Remove, 1)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, ja, new JsonValueChangeEventArgs(elementInArray, JsonValueChange.Remove, 1)),
                });
        }

        [TestMethod]
        public void NestedChangingEventTest()
        {
            JsonArray target = new JsonArray { new JsonArray { 1, 2 }, new JsonArray { 3, 4 } };
            JsonArray child = target[1] as JsonArray;
            TestEvents(
                target,
                arr => ((JsonArray)arr[1]).Add(5),
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>());

            target = new JsonArray();
            child = new JsonArray(1, 2);
            TestEvents(
                target,
                arr => {
                    arr.Add(child);
                    ((JsonArray)arr[0]).Add(5);
                },
                new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>
                {
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, target, new JsonValueChangeEventArgs(child, JsonValueChange.Add, 0)),
                    new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, target, new JsonValueChangeEventArgs(child, JsonValueChange.Add, 0)),
                });
        }

        [TestMethod]
        public void MultipleListenersTest()
        {
            for (int changingListeners = 0; changingListeners <= 2; changingListeners++)
            {
                for (int changedListeners = 0; changedListeners <= 2; changedListeners++)
                {
                    MultipleListenersTest<JsonArray>(
                        () => new JsonArray(1, 2),
                        delegate(JsonArray arr)
                        {
                            arr[1] = "hello";
                            arr.RemoveAt(0);
                            arr.Add("world");
                            arr.Clear();
                        },
                        new List<JsonValueChangeEventArgs>
                        {
                            new JsonValueChangeEventArgs("hello", JsonValueChange.Replace, 1),
                            new JsonValueChangeEventArgs(1, JsonValueChange.Remove, 0),
                            new JsonValueChangeEventArgs("world", JsonValueChange.Add, 1),
                            new JsonValueChangeEventArgs(null, JsonValueChange.Clear, 0),
                        },
                        new List<JsonValueChangeEventArgs>
                        {
                            new JsonValueChangeEventArgs(2, JsonValueChange.Replace, 1),
                            new JsonValueChangeEventArgs(1, JsonValueChange.Remove, 0),
                            new JsonValueChangeEventArgs("world", JsonValueChange.Add, 1),
                            new JsonValueChangeEventArgs(null, JsonValueChange.Clear, 0),
                        },
                        changingListeners,
                        changedListeners);
                }
            }
        }

        [TestMethod]
        public void JsonTypeTest()
        {
            JsonArray target = AnyInstance.AnyJsonArray;
            Assert.AreEqual(JsonType.Array, target.JsonType);
        }

        internal static void TestEvents<JsonValueType>(JsonValueType target, Action<JsonValueType> actionToTriggerEvent, List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> expectedEvents) where JsonValueType : JsonValue
        {
            List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> actualEvents = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>();
            EventHandler<JsonValueChangeEventArgs> changingHandler = delegate(object sender, JsonValueChangeEventArgs e)
            {
                actualEvents.Add(new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, sender as JsonValue, e));
            };

            EventHandler<JsonValueChangeEventArgs> changedHandler = delegate(object sender, JsonValueChangeEventArgs e)
            {
                actualEvents.Add(new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, sender as JsonValue, e));
            };

            target.Changing += new EventHandler<JsonValueChangeEventArgs>(changingHandler);
            target.Changed += new EventHandler<JsonValueChangeEventArgs>(changedHandler);

            actionToTriggerEvent(target);

            target.Changing -= new EventHandler<JsonValueChangeEventArgs>(changingHandler);
            target.Changed -= new EventHandler<JsonValueChangeEventArgs>(changedHandler);

            ValidateExpectedEvents(expectedEvents, actualEvents);
        }

        static void TestEvents(JsonArray array, Action<JsonArray> actionToTriggerEvent, List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> expectedEvents)
        {
            TestEvents<JsonArray>(array, actionToTriggerEvent, expectedEvents);
        }

        internal static void ValidateExpectedEvents(List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> expectedEvents, List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> actualEvents)
        {
            Assert.AreEqual(expectedEvents.Count, actualEvents.Count);
            for (int i = 0; i < expectedEvents.Count; i++)
            {
                bool expectedIsChanging = expectedEvents[i].Item1;
                bool actualIsChanging = expectedEvents[i].Item1;
                Assert.AreEqual(expectedIsChanging, actualIsChanging, string.Format("Event raised is different for expected event {0}: expected Chang{1}, actual Chang{2}", i, expectedIsChanging ? "ing" : "ed", actualIsChanging ? "ing" : "ed"));

                JsonValue expectedSender = expectedEvents[i].Item2;
                JsonValue actualSender = actualEvents[i].Item2;
                Assert.AreSame(expectedSender, actualSender, string.Format("Sender is different for expected event {0}: expected {1}, actual {2}", i, expectedSender, actualSender));

                JsonValueChangeEventArgs expectedEventArgs = expectedEvents[i].Item3;
                JsonValueChangeEventArgs actualEventArgs = actualEvents[i].Item3;
                Assert.AreEqual(expectedEventArgs.Change, actualEventArgs.Change, string.Format("JVChangeEventArgs.Change is different for event {0}: expected {1}, actual {2}", i, expectedEventArgs.Change, actualEventArgs.Change));
                Assert.AreEqual(expectedEventArgs.Index, actualEventArgs.Index, string.Format("JVChangeEventArgs.Index is different for event {0}: expected {1}, actual {2}", i, expectedEventArgs.Index, actualEventArgs.Index));
                Assert.AreEqual(expectedEventArgs.Key, actualEventArgs.Key, string.Format("JVChangeEventArgs.Key is different for event {0}: expected {1}, actual {2}", i, expectedEventArgs.Key ?? "<<null>>", actualEventArgs.Key ?? "<<null>>"));

                string expectedChild = expectedEventArgs.Child == null ? "null" : expectedEventArgs.Child.ToString();
                string actualChild = actualEventArgs.Child == null ? "null" : actualEventArgs.Child.ToString();
                Assert.AreEqual(expectedChild, actualChild, string.Format("JVChangeEventArgs.Child is different for event {0}: expected {1}, actual {2}", i, expectedChild, actualChild));
            }
        }

        internal static void MultipleListenersTest<JsonValueType>(
            Func<JsonValueType> createTarget,
            Action<JsonValueType> actionToTriggerEvents,
            List<JsonValueChangeEventArgs> expectedChangingEventArgs,
            List<JsonValueChangeEventArgs> expectedChangedEventArgs,
            int changingListeners,
            int changedListeners) where JsonValueType : JsonValue
        {
            Console.WriteLine("Testing events on a {0} for {1} changING listeners and {2} changED listeners", typeof(JsonValueType).Name, changingListeners, changedListeners);
            JsonValueType target = createTarget();
            List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>[] actualChangingEvents = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>[changingListeners];
            List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>[] actualChangedEvents = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>[changedListeners];
            List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> expectedChangingEvents = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>(
                expectedChangingEventArgs.Select((args) => new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, target, args)));
            List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>> expectedChangedEvents = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>(
                expectedChangedEventArgs.Select((args) => new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, target, args)));

            for (int i = 0; i < changingListeners; i++)
            {
                actualChangingEvents[i] = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>();
                int index = i;
                target.Changing += delegate(object sender, JsonValueChangeEventArgs e)
                {
                    actualChangingEvents[index].Add(new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(true, sender as JsonValue, e));
                };
            }

            for (int i = 0; i < changedListeners; i++)
            {
                actualChangedEvents[i] = new List<Tuple<bool, JsonValue, JsonValueChangeEventArgs>>();
                int index = i;
                target.Changed += delegate(object sender, JsonValueChangeEventArgs e)
                {
                    actualChangedEvents[index].Add(new Tuple<bool, JsonValue, JsonValueChangeEventArgs>(false, sender as JsonValue, e));
                };
            }

            actionToTriggerEvents(target);

            for (int i = 0; i < changingListeners; i++)
            {
                Console.WriteLine("Validating Changing events for listener {0}", i);
                ValidateExpectedEvents(expectedChangingEvents, actualChangingEvents[i]);
            }

            for (int i = 0; i < changedListeners; i++)
            {
                Console.WriteLine("Validating Changed events for listener {0}", i);
                ValidateExpectedEvents(expectedChangedEvents, actualChangedEvents[i]);
            }
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
