// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using System.ServiceModel.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpParameterDescriptionCollectionTests
    {
        #region Constructor Tests

        [TestMethod]
        [Description("HttpParameterDescriptionCollection supports default ctor")]
        public void HttpParameterDescriptionCollection_Default_Ctor()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            Assert.AreEqual(0, coll.Count, "Expected default ctor to init to empty collection");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection can be initialized from empty list")]
        public void HttpParameterDescriptionCollection_Ctor_Empty_List()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection(Enumerable.Empty<HttpParameterDescription>().ToList());
            Assert.AreEqual(0, coll.Count, "Expected empty list to init to empty collection");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection throws for null ctor descriptions parameter")]
        public void HttpParameterDescriptionCollection_Ctor_Throws_Null_Descriptions()
        {
            ExceptionAssert.ThrowsArgumentNull(
                "Null list of descriptions should throw",
                "descriptions",
                () =>
                {
                    HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection(descriptions: null);
                });
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection can be constructed from mock HttpParameterDescriptions")]
        public void HttpParameterDescriptionCollection_Ctor_From_Mock_HttpParameterDescriptions()
        {
            HttpParameterDescription[] paramDescs = new[] {
                new HttpParameterDescription() { Name="First", Namespace="FirstNS", ParameterType=typeof(string), Index=0 },
                new HttpParameterDescription() { Name="Second", Namespace="SecondNS", ParameterType=typeof(int), Index=1 },
            };

            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection(paramDescs.ToList());
            Assert.AreEqual(2, coll.Count, "HttpParameterDescriptionCollection should have found 2 parameters");

            Assert.AreEqual("First", coll[0].Name, "Name1 was not set correctly");
            Assert.AreEqual("FirstNS", coll[0].Namespace, "Namespace1 was not set correctly");
            Assert.AreEqual(typeof(string), coll[0].ParameterType, "Type1 was not set correctly");
            Assert.AreEqual(0, coll[0].Index, "Index1 was not set correctly");

            Assert.AreEqual("Second", coll[1].Name, "Name2 was not set correctly");
            Assert.AreEqual("SecondNS", coll[1].Namespace, "Namespace2 was not set correctly");
            Assert.AreEqual(typeof(int), coll[1].ParameterType, "Type2 was not set correctly");
            Assert.AreEqual(1, coll[1].Index, "Index2 was not set correctly");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection can be constructed from input MessagePartDescriptionCollection")]
        public void HttpParameterDescriptionCollection_Ctor_From_Input_MessagePartDescriptionCollection()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleMethod");
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            Assert.IsNotNull(coll, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(1, coll.Count, "HttpParameterDescriptionCollection should have found 1 parameter");
            HttpParameterDescription hpd = coll[0];

            Assert.AreEqual("x", hpd.Name, "Name was not set correctly");
            Assert.AreEqual(typeof(int), hpd.ParameterType, "ParameterType was not set correctly");
            Assert.AreEqual(0, hpd.Index, "Index was not set correctly");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection can be constructed from output MessagePartDescriptionCollection")]
        public void HttpParameterDescriptionCollection_Ctor_From_Output_MessagePartDescriptionCollection()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection(od, isOutputCollection: true);
            Assert.IsNotNull(coll, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(1, coll.Count, "HttpParameterDescriptionCollection should have found 1 parameter");
            HttpParameterDescription hpd = coll[0];
           
            Assert.AreEqual("outParameter", hpd.Name, "Name was not set correctly");
            Assert.AreEqual(typeof(double), hpd.ParameterType, "ParameterType was not set correctly");
            Assert.AreEqual(0, hpd.Index, "Index was not set correctly");
        }

        #endregion Constructor Tests

        #region Update Unsynchronized Tests

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements Clear correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_Implements_Clear()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // Clear
            coll.Clear();
            Assert.AreEqual(0, coll.Count, "Clear failed");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements indexer correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_Indexer()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };
            HttpParameterDescription hpdTemp = null;

            coll.Add(hpd1);
            coll.Add(hpd2);

            // Indexer get
            Assert.AreEqual(2, coll.Count, "Count incorrect");
            Assert.AreSame(hpd1, coll[0], "Indexer[0] incorrect");
            Assert.AreSame(hpd2, coll[1], "Indexer[1] incorrect");

            // Indexer get negative
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Indexer should throw for too large index",
                () => hpdTemp = coll[2],
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Indexer should throw for negative index",
                () => hpdTemp = coll[-1],
                "index"
                );

            // Indexer set
            coll[1] = hpd3;
            Assert.AreSame(hpd3, coll[1], "Indexer set failed");

            // Indexer set null item
            ExceptionAssert.ThrowsArgumentNull(
                "Indexer set with null should throw",
                "value",
                () => coll[0] = null);

            // Indexer set negative
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Indexer should throw for too large index",
                () => coll[5] = hpd2,
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Indexer should throw for negative index",
                () => coll[-1] = hpd2,
                "index"
                );
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements IndexOf correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_IndexOf()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // IndexOf
            Assert.AreEqual(0, coll.IndexOf(hpd1), "IndexOf[0] incorrect");
            Assert.AreEqual(1, coll.IndexOf(hpd2), "IndexOf[1] incorrect");
            Assert.AreEqual(-1, coll.IndexOf(hpd3), "IndexOf[none] incorrect");

            // IndexOf negative
            ExceptionAssert.ThrowsArgumentNull(
                "IndexOf with null should throw",
                "item",
                () => coll.IndexOf(null));
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements Contains correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_Contains()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // Contains
            Assert.IsTrue(coll.Contains(hpd1), "Contains[0] incorrect");
            Assert.IsTrue(coll.Contains(hpd2), "Contains[1] incorrect");
            Assert.IsFalse(coll.Contains(hpd3), "Contains[none] incorrect");

            // Contains negative
            ExceptionAssert.ThrowsArgumentNull(
                "IndexOf with null should throw",
                "item",
                () => coll.Contains(null));
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements IsReadOnly correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_IsReadOnly()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            Assert.IsFalse(coll.IsReadOnly, "Collection should not be readonly");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements CopyTo correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_CopyTo()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // CopyTo
            HttpParameterDescription[] arr = new HttpParameterDescription[2];
            coll.CopyTo(arr, 0);
            Assert.AreSame(hpd1, arr[0], "CopyTo[0] failed");
            Assert.AreSame(hpd2, arr[1], "CopyTo[1] failed");

            // CopyTo negative tests
            ExceptionAssert.ThrowsArgumentNull(
                "CopyTo throws argument null for null array",
                "array",
                () => coll.CopyTo(null, 0)
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "CopyTo should throw for negative index",
                () => coll.CopyTo(arr, -1),
                "arrayIndex"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "CopyTo should throw for too large an index",
                () => coll.CopyTo(arr, 2),
                "arrayIndex"
                );
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements Insert correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_Insert()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            // Insert semamtics allow index==Count.  Verify.
            coll.Insert(0, hpd1);
            coll.Insert(1, hpd2);

            // Now really insert between
            coll.Insert(1, hpd3);

            Assert.AreEqual(3, coll.Count, "Insert failed");
            Assert.AreSame(hpd3, coll[1], "Insert went to wrong spot");
            Assert.AreSame(hpd2, coll[2], "Insert did not move items");

            // Insert negative
            ExceptionAssert.ThrowsArgumentNull(
                "Insert throws argument null for null item",
                "item",
                () => coll.Insert(0, null)
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Insert should throw for negative index",
                () => coll.Insert(-1, hpd3),
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Insert should throw for too large index",
                () => coll.Insert(4, hpd3),
                "index"
                );

        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements Remove correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_Remove()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // Remove
            coll.Remove(hpd3);
            Assert.AreEqual(2, coll.Count, "Remove failed");
            Assert.IsFalse(coll.Contains(hpd3), "Remove still shows contains");

            // Remove negative
            Assert.IsFalse(coll.Remove(hpd3), "Redundant remove should have returned false");

            ExceptionAssert.ThrowsArgumentNull(
                "Remove throws argument null for null item",
                "item",
                () => coll.Remove(null)
                );

        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements RemoveAt correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_RemoveAt()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // RemoveAt
            coll.Add(hpd3);
            Assert.AreEqual(3, coll.Count, "Add failed");
            Assert.IsTrue(coll.Contains(hpd3), "Contains after add failed");
            coll.RemoveAt(2);
            Assert.AreEqual(2, coll.Count, "RemoveAt count failed");
            Assert.IsFalse(coll.Contains(hpd3), "RemoveAt+Contains failed");

            // RemoveAt negative
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "RemoveAt should throw for negative index",
                () => coll.RemoveAt(-1),
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "RemoveAt should throw for too large index",
                () => coll.RemoveAt(3),
                "index"
                );
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from default ctor implements GetEnumerator correctly")]
        public void HttpParameterDescriptionCollection_Unsynchronized_GetEnumerator()
        {
            HttpParameterDescriptionCollection coll = new HttpParameterDescriptionCollection();
            HttpParameterDescription hpd1 = new HttpParameterDescription()
            {
                Name = "First",
                Namespace = "FirstNS",
                Index = 0,
                ParameterType = typeof(string)
            };
            HttpParameterDescription hpd2 = new HttpParameterDescription()
            {
                Name = "Second",
                Namespace = "SecondNS",
                Index = 1,
                ParameterType = typeof(int)
            };
            HttpParameterDescription hpd3 = new HttpParameterDescription()
            {
                Name = "Third",
                Namespace = "ThirdNS",
                Index = 2,
                ParameterType = typeof(double)
            };

            coll.Add(hpd1);
            coll.Add(hpd2);

            // GetEnumerator
            IEnumerator<HttpParameterDescription> ie = coll.GetEnumerator();
            Assert.IsNotNull(ie, "GetEnumerator failed");
            object[] items = EnumeratorToArray(ie);
            AssertSame(coll, items, "Generic enumerator");

            // Non-generic GetEnumerator
            IEnumerator iec = ((IEnumerable)coll).GetEnumerator();
            Assert.IsNotNull(iec, "GetEnumerator failed");
            items = EnumeratorToArray(iec);
            AssertSame(coll, items, "Non-generic enumerator");
        }

        #endregion Update Unsynchronized Tests

        #region Update Synchronized with MessagePartDescription Tests

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements Clear correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Implements_Clear()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);

            hpdColl.Clear();
            Assert.AreEqual(0, hpdColl.Count, "Clear failed");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements Indexer correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Indexer()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            HttpParameterDescription hpdTemp = null;

            // Indexer get (note this verifies HPD indexer redirects to original MPD coll
            Assert.AreEqual(2, hpdColl.Count, "Count incorrect");
            Assert.AreSame(hpd1.MessagePartDescription, hpdColl[0].MessagePartDescription, "Indexer[0] incorrect");
            Assert.AreSame(hpd2.MessagePartDescription, hpdColl[1].MessagePartDescription, "Indexer[1] incorrect");

            // Indexer get negative
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Indexer should throw for negative index",
                () => hpdTemp = hpdColl[2],
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Indexer should throw for negative index",
                () => hpdTemp = hpdColl[-1],
                "index"
                );

            // Indexer set
            hpdColl[1] = hpd3;
            Assert.AreEqual(2, hpdColl.Count, "Index set should have not affected count");
            Assert.AreSame(hpd3.MessagePartDescription, hpdColl[1].MessagePartDescription, "Indexer[1] set incorrect");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements IndexOf correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_IndexOf()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            // IndexOf
            Assert.AreEqual(0, hpdColl.IndexOf(hpd1), "IndexOf[0] incorrect");
            Assert.AreEqual(1, hpdColl.IndexOf(hpd2), "IndexOf[1] incorrect");
            Assert.AreEqual(-1, hpdColl.IndexOf(hpd3), "IndexOf[none] incorrect");

            // IndexOf negative
            ExceptionAssert.ThrowsArgumentNull(
                "IndexOf with null should throw",
                "item",
                () => hpdColl.IndexOf(null));
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements Contains correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Implements_Contains()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            // Contains
            Assert.IsTrue(hpdColl.Contains(hpd1), "Contains[0] incorrect");
            Assert.IsTrue(hpdColl.Contains(hpd2), "Contains[1] incorrect");
            Assert.IsFalse(hpdColl.Contains(hpd3), "Contains[none] incorrect");

            // Contains negative
            ExceptionAssert.ThrowsArgumentNull(
                "IndexOf with null should throw",
                "item",
                () => hpdColl.Contains(null));
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements IsReadOnly correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_IsReadOnly()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements CopyTo correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_CopyTo()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            // CopyTo
            HttpParameterDescription[] arr = new HttpParameterDescription[2];
            hpdColl.CopyTo(arr, 0);
            Assert.AreSame(hpd1.MessagePartDescription, arr[0].MessagePartDescription, "CopyTo[0] failed");
            Assert.AreSame(hpd2.MessagePartDescription, arr[1].MessagePartDescription, "CopyTo[1] failed");

            // CopyTo negative tests
            ExceptionAssert.ThrowsArgumentNull(
                "CopyTo throws argument null for null array",
                "array",
                () => hpdColl.CopyTo(null, 0)
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "CopyTo should throw for negative index",
                () => hpdColl.CopyTo(arr, -1),
                "arrayIndex"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "CopyTo should throw for too large an index",
                () => hpdColl.CopyTo(arr, 2),
                "arrayIndex"
                );
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements Insert correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Insert()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            // Insert
            hpdColl.Insert(1, hpd3);
            Assert.AreEqual(3, hpdColl.Count, "Insert failed");
            Assert.AreSame(hpd3.MessagePartDescription, hpdColl[1].MessagePartDescription, "Insert went to wrong spot");
            Assert.AreSame(hpd2.MessagePartDescription, hpdColl[2].MessagePartDescription, "Insert did not move items");

            // Insert negative
            ExceptionAssert.ThrowsArgumentNull(
                "Insert throws argument null for null item",
                "item",
                () => hpdColl.Insert(0, null)
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Insert should throw for negative index",
                () => hpdColl.Insert(-1, hpd3),
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Insert should throw for too large index",
                () => hpdColl.Insert(4, hpd3),
                "index"
                );

        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements Remove correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Remove()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();


            // Remove
            hpdColl.Remove(hpd3);
            Assert.AreEqual(2, hpdColl.Count, "Remove failed");
            Assert.IsFalse(hpdColl.Contains(hpd3), "Remove still shows contains");

            // Remove negative
            Assert.IsFalse(hpdColl.Remove(hpd3), "Redundant remove should have returned false");

            ExceptionAssert.ThrowsArgumentNull(
                "Remove throws argument null for null item",
                "item",
                () => hpdColl.Remove(null)
                );
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements RemoveAt correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_RemoveAt()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            // RemoveAt
            hpdColl.Add(hpd3);
            Assert.AreEqual(3, hpdColl.Count, "Add failed");
            Assert.IsTrue(hpdColl.Contains(hpd3), "Contains after add failed");
            hpdColl.RemoveAt(2);
            Assert.AreEqual(2, hpdColl.Count, "RemoveAt count failed");
            Assert.IsFalse(hpdColl.Contains(hpd3), "RemoveAt+Contains failed");

            // RemoveAt negative
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "RemoveAt should throw for negative index",
                () => hpdColl.RemoveAt(-1),
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "RemoveAt should throw for too large index",
                () => hpdColl.RemoveAt(3),
                "index"
                );
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from MessagePartDescriptionCollection implements GetEnumerator correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_GetEnumerator()
        {
            OperationDescription od1 = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            OperationDescription od2 = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescriptionCollection mpdColl = od1.Messages[0].Body.Parts;
            Assert.AreEqual(2, mpdColl.Count, "MessagePartDescriptionCollection should show 2 existing input parameters");

            MessagePartDescriptionCollection mpdColl2 = od2.Messages[0].Body.Parts;
            Assert.AreEqual(1, mpdColl2.Count, "MessagePartDescriptionCollection 2 should show 1 existing input parameters");

            // Pull out individual parts to test synching at item level
            MessagePartDescription mpd1 = mpdColl[0];
            MessagePartDescription mpd2 = mpdColl[1];

            // Use a MPD from a 2nd collection so we can add and remove it
            MessagePartDescription mpd3 = mpdColl2[0];

            // This ctor creates the synchronized form of the collection.   It should immediately reflect
            // the state of the MPD collection
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od1, isOutputCollection: false);
            Assert.IsNotNull(hpdColl, "Failed to create HttpParameterDescriptionCollection");
            Assert.AreEqual(2, hpdColl.Count, "HttpParameterDescriptionCollection should show 2 existing input parameters");

            // Extension method creates synched version of HPD from MPD's
            HttpParameterDescription hpd1 = mpd1.ToHttpParameterDescription();
            HttpParameterDescription hpd2 = mpd2.ToHttpParameterDescription();

            // Ensure the extension method created HPD's that point to the idential MPD
            Assert.AreEqual(mpd1, hpd1.MessagePartDescription, "HttParameterDescription 1 linked to wrong MessagePartDescription");
            Assert.AreEqual(mpd2, hpd2.MessagePartDescription, "HttParameterDescription 2 linked to wrong MessagePartDescription");

            // Keep one from 2nd collection
            HttpParameterDescription hpd3 = mpd3.ToHttpParameterDescription();

            // GetEnumerator
            IEnumerator<HttpParameterDescription> ie = hpdColl.GetEnumerator();
            object[] items = EnumeratorToArray(ie);
            AssertSame(hpdColl, items, "Generic enumerator");

            // Non-generic GetEnumerator
            IEnumerator iec = ((IEnumerable)hpdColl).GetEnumerator();
            Assert.IsNotNull(iec, "GetEnumerator failed");
            items = EnumeratorToArray(iec);
            AssertSame(hpdColl, items, "Nongeneric enumerator");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection throws if attempt to add or insert unsynchronized instance to synchronized collection.")]
        public void HttpParameterDescriptionCollection_Synchronized_Collection_Throws_New_Mock_HttpParameterDescriptions()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            MessagePartDescriptionCollection mpdColl = od.Messages[0].Body.Parts;
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            Assert.IsFalse(hpdColl.IsReadOnly, "Collection should not be readonly");

            // Create a new HPD from simple types
            HttpParameterDescription hpd = new HttpParameterDescription()
            {
                Name = "MockHpd",
                Namespace = "MockHpdNS",
                Index = 2,
                ParameterType = this.GetType()
            };

            ExceptionAssert.ThrowsInvalidOperation(
                "Should throw if attempt to add unsynchronized item to synchronized collection",
                () => hpdColl.Add(hpd));

            ExceptionAssert.ThrowsInvalidOperation(
                "Should throw if attempt to insert unsynchronized item to synchronized collection",
                () => hpdColl.Insert(0, hpd));

            ExceptionAssert.ThrowsInvalidOperation(
                "Should throw if attempt to test contains of unsynchronized item to synchronized collection",
                () => hpdColl.Contains(hpd));

            ExceptionAssert.ThrowsInvalidOperation(
                "Should throw if attempt to test contains of unsynchronized item to synchronized collection",
                () => hpdColl.IndexOf(hpd));
        }

        #endregion Update Synchronized with MessagePartDescription Tests

        #region Update Synchronized with incomplete MessagePartDescription Tests

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements Clear correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_Implements_Clear()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            od.Messages.Clear();
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            hpdColl.Clear();
            Assert.AreEqual(0, hpdColl.Count, "Clear failed");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements Indexer correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_Indexer()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            HttpParameterDescription hpd = hod.InputParameters[0];

            // Zap both inputs and outputs
            MessageDescription mdInput = od.Messages[0];
            MessageDescription mdOutput = od.Messages[1];
            od.Messages.Clear();

            // Verify the HOD sees an empty set
            Assert.AreEqual(0, hod.InputParameters.Count, "Expected zero input parameters");

            // Verify our local collection sees an empty set
            Assert.AreEqual(0, hpdColl.Count, "Expected zero elements in local collection");

            // Expect ArgumentOutOfRangeException using get indexer
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Expected ArgumentOutOfRangeException indexing into missing inputs",
                () => hpd = hod.InputParameters[0]
                );

            // Same exception indexing our local collection
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Expected ArgumentOutOfRangeException indexing into missing inputs",
                () => hpd = hpdColl[0]
                );

            // Expect ArgumentOutOfRangeException using set indexer on InputParameters
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Expected ArgumentOutOfRangeException indexing into missing inputs",
                () => hod.InputParameters[0] = hpd
                );

            // Same exception setting on our local collection
            ExceptionAssert.Throws(
                 typeof(ArgumentOutOfRangeException),
                 "Expected ArgumentOutOfRangeException indexing into missing inputs",
                 () => hpdColl[0] = hpd
                 );

            // Add back a real input MessageDescription and verify collections have content again
            od.Messages.Add(mdInput);

            Assert.AreEqual(2, hod.InputParameters.Count, "HOD.InputParameters did not update when added MessageDescription");
            Assert.AreEqual(2, hpdColl.Count, "HODColl.Count did not update when added MessageDescription");

            // The indexer get should work again
            HttpParameterDescription hpd1 = hpdColl[0];
            Assert.IsNotNull(hpd1, "Unexpected null reindexing collection");
            hpd1 = hod.InputParameters[0];
            Assert.IsNotNull(hpd1, "Unexpected null reindexing InputParameters");

            // And so should the setter indexer
            hpdColl[0] = hpd;
            hod.InputParameters[0] = hpd;
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements IndexOf correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_IndexOf()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            HttpParameterDescription hpd = hod.InputParameters[0];

            Assert.AreEqual(0, hpdColl.IndexOf(hpd), "Prove IndexOf works prior to clearing");

            // Zap both inputs and outputs
            MessageDescription mdInput = od.Messages[0];
            MessageDescription mdOutput = od.Messages[1];
            od.Messages.Clear();

            // Verify the HOD sees an empty set
            Assert.AreEqual(0, hod.InputParameters.Count, "Expected zero input parameters");
            Assert.AreEqual(0, hpdColl.Count, "Expected zero elements in local collection");

            // Verify IndexOf cannot find it in either collection
            Assert.AreEqual(-1, hod.InputParameters.IndexOf(hpd), "InputParameters.IndexOf should not have found it");
            Assert.AreEqual(-1, hpdColl.IndexOf(hpd), "HPDColl.IndexOf should not have found it");

            // Add back a real input MessageDescription and verify collections have content again
            od.Messages.Add(mdInput);

            Assert.AreEqual(0, hod.InputParameters.IndexOf(hpd), "InputParameters.IndexOf should have found it");
            Assert.AreEqual(0, hpdColl.IndexOf(hpd), "HPDColl.IndexOf should have found it");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements Contains correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_Implements_Contains()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            HttpParameterDescription hpd = hod.InputParameters[0];

            Assert.IsTrue(hpdColl.Contains(hpd), "Prove Contains works prior to clearing");

            // Zap both inputs and outputs
            MessageDescription mdInput = od.Messages[0];
            MessageDescription mdOutput = od.Messages[1];
            od.Messages.Clear();

            // Verify Contains cannot find it in either collection
            Assert.IsFalse(hod.InputParameters.Contains(hpd), "InputParameters.Contains should not have found it");
            Assert.IsFalse(hpdColl.Contains(hpd), "HPDColl.Contains should not have found it");

            // Add back a real input MessageDescription and verify collections have content again
            od.Messages.Add(mdInput);

            Assert.IsTrue(hod.InputParameters.Contains(hpd), "InputParameters.Contains should have found it");
            Assert.IsTrue(hpdColl.Contains(hpd), "HPDColl.Contains should have found it");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements IsReadOnly correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_IsReadOnly()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            od.Messages.Clear();
            Assert.IsFalse(hod.InputParameters.IsReadOnly, "Input should should as not readonly regardless of sync");
            Assert.IsFalse(hod.OutputParameters.IsReadOnly, "Output should should as not readonly regardless of sync");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements CopyTo correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_CopyTo()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            HttpParameterDescriptionCollection hpdColl = new HttpParameterDescriptionCollection(od, isOutputCollection: false);
            HttpParameterDescription hpd = hod.InputParameters[0];

            Assert.IsTrue(hpdColl.Contains(hpd), "Prove Contains works prior to clearing");

            // Zap both inputs and outputs
            MessageDescription mdInput = od.Messages[0];
            MessageDescription mdOutput = od.Messages[1];
            od.Messages.Clear();

            HttpParameterDescription[] arr = new HttpParameterDescription[2];

            // CopyTo should throw ArgumentOutOfRange for any copy request
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "Expected ArgumentOutOfRangeException on empty Messages",
                () => hpdColl.CopyTo(arr, 0),
                "arrayIndex"
            );

            od.Messages.Add(mdInput);

            hpdColl.CopyTo(arr, 0);
            Assert.AreEqual(hpd.MessagePartDescription, arr[0].MessagePartDescription, "Copy did not yield expected instance");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements Insert correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_Insert()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();

            // Pull collections into locals
            HttpParameterDescriptionCollection hpdCollInput = hod.InputParameters;
            HttpParameterDescriptionCollection hpdCollOutput = hod.OutputParameters;

            HttpParameterDescription hpdInput = hpdCollInput[0];
            HttpParameterDescription hpdOutput = hpdCollOutput[0];

            // Zap the Messages collection
            od.Messages.Clear();

            Assert.AreEqual(0, hpdCollInput.Count, "Clearing Messages should have reset input count");
            Assert.AreEqual(0, hpdCollOutput.Count, "Clearing Messages should have reset output count");

            // Inserting into output should autocreate both Messages
            // This also verifies insert where index==Count which is legal
            hpdCollOutput.Insert(0, hpdOutput);

            Assert.AreEqual(1, hpdCollOutput.Count, "Failed to insert output");
            Assert.AreEqual(2, od.Messages.Count, "Failed to autocreate Messages[1]");

            // Should be possible to insert input again too
            hpdCollInput.Insert(0, hpdInput);
            Assert.AreEqual(1, hpdCollInput.Count, "Failed to insert input");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements Remove correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_Remove()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            // Pull collections into locals
            HttpParameterDescriptionCollection hpdCollInput = hod.InputParameters;
            HttpParameterDescriptionCollection hpdCollOutput = hod.OutputParameters;

            HttpParameterDescription hpdInput = hpdCollInput[0];
            HttpParameterDescription hpdOutput = hpdCollOutput[0];

            // Zap the Messages collection
            od.Messages.Clear();

            Assert.AreEqual(0, hpdCollInput.Count, "Clearing Messages should have reset input count");
            Assert.AreEqual(0, hpdCollOutput.Count, "Clearing Messages should have reset output count");

            // Remove
            bool removed = hpdCollInput.Remove(hpdOutput);
            Assert.IsFalse(removed, "Remove of input should have returned false");
            
            removed = hpdCollOutput.Remove(hpdOutput);
            Assert.IsFalse(removed, "Remove of output should have returned false");

            // Put it back to verify Remove can work after recreate Messages
            hpdCollOutput.Add(hpdOutput);

            Assert.AreEqual(2, od.Messages.Count, "Expected Messages to be autocreated");

            removed = hpdCollOutput.Remove(hpdOutput);
            Assert.IsTrue(removed, "Remove of output after autogen should have returned true");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements RemoveAt correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_RemoveAt()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            // Pull collections into locals
            HttpParameterDescriptionCollection hpdCollInput = hod.InputParameters;
            HttpParameterDescriptionCollection hpdCollOutput = hod.OutputParameters;

            HttpParameterDescription hpdInput = hpdCollInput[0];
            HttpParameterDescription hpdOutput = hpdCollOutput[0];

            // Zap the Messages collection
            od.Messages.Clear();

            Assert.AreEqual(0, hpdCollInput.Count, "Clearing Messages should have reset input count");
            Assert.AreEqual(0, hpdCollOutput.Count, "Clearing Messages should have reset output count");

            // RemoveAt should throw on empty collections
            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "RemoveAt should throw when nothing in collection",
                () => hpdCollInput.RemoveAt(0),
                "index"
                );

            ExceptionAssert.Throws(
                typeof(ArgumentOutOfRangeException),
                "RemoveAt should throw when nothing in collection",
                () => hpdCollOutput.RemoveAt(0),
                "index"
                );

            // Put it back to verify RemoveAt can work after recreate Messages
            hpdCollOutput.Add(hpdOutput);
            hpdCollInput.Add(hpdInput);

            Assert.AreEqual(2, od.Messages.Count, "Expected Messages to be autocreated");
            Assert.AreEqual(1, hpdCollInput.Count, "Expected input coll count to be 1");
            Assert.AreEqual(1, hpdCollOutput.Count, "Expected output coll count to be 1");

            // RemoveAt should work again
            hpdCollInput.RemoveAt(0);
            hpdCollOutput.RemoveAt(0);

            Assert.AreEqual(0, hpdCollInput.Count, "Expected input coll count to be 0");
            Assert.AreEqual(0, hpdCollOutput.Count, "Expected output coll count to be ");
        }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements generic GetEnumerator correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_Generic_GetEnumerator()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            // Pull collections into locals
            HttpParameterDescriptionCollection hpdCollInput = hod.InputParameters;
            HttpParameterDescriptionCollection hpdCollOutput = hod.OutputParameters;

            HttpParameterDescription hpdInput = hpdCollInput[0];
            HttpParameterDescription hpdOutput = hpdCollOutput[0];

            // Zap the Messages collection
            od.Messages.Clear();

            int nItems = 0;
            using (IEnumerator<HttpParameterDescription> ie = hpdCollInput.GetEnumerator())
            {
                Assert.IsNotNull(ie, "enumerator should not be null");
                nItems = 0;
                while (ie.MoveNext())
                {
                    ++nItems;
                }
            }
            Assert.AreEqual(0, nItems, "Input enumerator should have been empty");

            using (IEnumerator<HttpParameterDescription> ie = hpdCollOutput.GetEnumerator())
            {
                Assert.IsNotNull(ie, "enumerator should not be null");
                nItems = 0;
                while (ie.MoveNext())
                {
                    ++nItems;
                }
            }
            Assert.AreEqual(0, nItems, "Output enumerator should have been empty");

            // Add back the HPD's and verify enumerator works again
            hpdCollInput.Add(hpdInput);
            hpdCollOutput.Add(hpdOutput);

            using (IEnumerator<HttpParameterDescription> ie = hpdCollInput.GetEnumerator())
            {
                Assert.IsNotNull(ie, "input enumerator should not be null");
                nItems = 0;
                while (ie.MoveNext())
                {
                    ++nItems;
                }
            }
            Assert.AreEqual(1, nItems, "Input enumerator should have had one item");

            using (IEnumerator<HttpParameterDescription> ie = hpdCollOutput.GetEnumerator())
            {
                Assert.IsNotNull(ie, "output enumerator should not be null");
                nItems = 0;
                while (ie.MoveNext())
                {
                    ++nItems;
                }
            }
            Assert.AreEqual(1, nItems, "Output enumerator should have had one item");
         }

        [TestMethod]
        [Description("HttpParameterDescriptionCollection created from incomplete MessagePartDescriptionCollection implements GetEnumerator correctly")]
        public void HttpParameterDescriptionCollection_Synchronized_Incomplete_GetEnumerator()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpOperationDescription hod = od.ToHttpOperationDescription();
            // Pull collections into locals
            HttpParameterDescriptionCollection hpdCollInput = hod.InputParameters;
            HttpParameterDescriptionCollection hpdCollOutput = hod.OutputParameters;

            HttpParameterDescription hpdInput = hpdCollInput[0];
            HttpParameterDescription hpdOutput = hpdCollOutput[0];

            // Zap the Messages collection
            od.Messages.Clear();

            // Empty enumerators should match
            IEnumerator ie = ((IEnumerable)hpdCollInput).GetEnumerator();
            Assert.IsNotNull(ie, "enumerator should not be null");
            object[] items = EnumeratorToArray(ie);
            Assert.AreEqual(0, items.Length, "Expected empty enumerator for input");

            ie = ((IEnumerable)hpdCollOutput).GetEnumerator();
            Assert.IsNotNull(ie, "enumerator should not be null");
            items = EnumeratorToArray(ie);
            Assert.AreEqual(0, items.Length, "Expected empty enumerator for output");

            // Add back the HPD's and verify enumerator works again
            hpdCollInput.Add(hpdInput);
            hpdCollOutput.Add(hpdOutput);

            ie = ((IEnumerable)hpdCollInput).GetEnumerator();
            Assert.IsNotNull(ie, "input enumerator should not be null");
            items = EnumeratorToArray(ie);
            Assert.AreEqual(1, items.Length, "Expected nonempty enumerator for input");
            AssertSame(hpdCollInput, items, "Generic input");

            ie = ((IEnumerable)hpdCollOutput).GetEnumerator();
            Assert.IsNotNull(ie, "output enumerator should not be null");
            items = EnumeratorToArray(ie);
            Assert.AreEqual(1, items.Length, "Expected nonempty enumerator for output");
            AssertSame(hpdCollOutput, items, "Generic input");
        }

        #endregion Update Synchronized with incomplete MessagePartDescription Tests

        #region helper

        private static object[] EnumeratorToArray(IEnumerator ie)
        {
            List<object> result = new List<object>();
            while (ie.MoveNext())
            {
                result.Add(ie.Current);
            }
            return result.ToArray();
        }

        private static void AssertSame(HttpParameterDescriptionCollection coll, object[] items, string message)
        {
            Assert.AreEqual(coll.Count, items.Length, message + ": length mismatch");
            for (int i = 0; i < items.Length; ++i)
            {
                HttpParameterDescription hpd1 = coll[i];
                HttpParameterDescription hpd2 = items[i] as HttpParameterDescription;
                Assert.IsNotNull(hpd1, message + ": null in collection");
                Assert.IsNotNull(hpd2, message + ": null in items");
                Assert.AreSame(hpd1.MessagePartDescription, hpd2.MessagePartDescription, message + ": different MessagePartDescriptions");
            }
        }

        public static OperationDescription GetOperationDescription(Type contractType, string methodName)
        {
            ContractDescription cd = ContractDescription.GetContract(contractType);
            OperationDescription od = cd.Operations.FirstOrDefault(o => o.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(od, "Failed to get operation description for " + methodName);
            return od;
        }
        #endregion helper
    }
}
