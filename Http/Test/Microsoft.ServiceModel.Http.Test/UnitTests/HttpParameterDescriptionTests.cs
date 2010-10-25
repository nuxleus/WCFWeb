// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Linq;
    using System.ServiceModel.Description;
    using System.ServiceModel.Http.Test.Mocks;
    using System.ServiceModel.Http.Test.Utilities;
    using System.ServiceModel.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpParameterDescriptionTests
    {
        #region Constructor Tests

        [TestMethod]
        [Description("HttpParameterDescription supports default ctor")]
        public void HttpParameterDescription_Default_Ctor()
        {
            HttpParameterDescription hpd = new HttpParameterDescription();
            Assert.IsNull(hpd.ParameterType, "ProcessorType should have been null");
            Assert.IsNull(hpd.Name, "Name should have been null");
            Assert.IsNull(hpd.Namespace, "Namespace should have been null");
            Assert.AreEqual(0, hpd.Index, "Index should have been zero");
            Assert.IsNull(hpd.MessagePartDescription, "Internal messagePartDescription should be null");
        }

        [TestMethod]
        [Description("HttpParameterDescription can be constructed from simple properties")]
        public void HttpParameterDescription_Construct_From_Simple_Properties()
        {
            HttpParameterDescription hpd = new HttpParameterDescription()
            {
                Name = "Sample",
                Namespace = "SampleNS",
                ParameterType = typeof(string),
                Index = 1
            };

            Assert.AreEqual("Sample", hpd.Name, "Name was not set correctly");
            Assert.AreEqual("SampleNS", hpd.Namespace, "Namespace was not set correctly");
            Assert.AreEqual(typeof(string), hpd.ParameterType, "ProcessorType was not set correctly");
            Assert.AreEqual(1, hpd.Index, "Index was not set correctly");
        }

        [TestMethod]
        [Description("HttpParameterDescription can be constructed from MessagePartDescription")]
        public void HttpParameterDescription_Construct_From_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescription mpd = od.Messages[1].Body.ReturnValue;
            HttpParameterDescription hpd = new HttpParameterDescription(mpd);
       
            Assert.AreEqual("SampleMethodResult", hpd.Name, "Name was not set correctly");
            Assert.AreEqual(typeof(string), hpd.ParameterType, "ProcessorType was not set correctly");
            Assert.AreEqual(0, hpd.Index, "Index was not set correctly");
            Assert.AreSame(mpd, hpd.MessagePartDescription, "Internal messagePartDescription should be what we passed to ctor");
        }

        #endregion Constructor Tests

        #region Extension method tests

        [TestMethod]
        [Description("HttpParameterDescription can be constructed via extension method from MessagePartDescription")]
        public void HttpParameterDescription_ExtensionMethod_Creates_From_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleMethod");
            MessagePartDescription mpd = od.Messages[1].Body.ReturnValue;
            HttpParameterDescription hpd = mpd.ToHttpParameterDescription();

            Assert.AreEqual("SampleMethodResult", hpd.Name, "Name was not set correctly");
            Assert.AreEqual(typeof(string), hpd.ParameterType, "ProcessorType was not set correctly");
            Assert.AreEqual(0, hpd.Index, "Index was not set correctly");
            Assert.AreSame(mpd, hpd.MessagePartDescription, "Internal messagePartDescription should be what we passed to ctor");
        }

        [TestMethod]
        [Description("HttpParameterDescription can be constructed via extension method from MessagePartDescription")]
        public void HttpParameterDescription_ExtensionMethod_Throws_Null_MessagePartDescription()
        {
            MessagePartDescription mpd = null;
            ExceptionAssert.ThrowsArgumentNull(
                "Null MessagePartDescription should throw",
                "description",
                () => mpd.ToHttpParameterDescription()
            );
        }

        #endregion Extension method tests

        #region Unsynchronized Tests

        #endregion Unsynchronized Tests

        #region Update HttpParameterDescription Tests
        [TestMethod]
        [Description("HttpParameterDescription Name property setter will throw when constructed from MessagePartDescription")]
        public void HttpParameterDescription_Name_Immutable_From_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpParameterDescription hpd = new HttpParameterDescription(od.Messages[0].Body.Parts[0]);
            ExceptionAssert.Throws(
                typeof(NotSupportedException),
                "Setting name property should throw",
                () => { hpd.Name = "newName"; }
                );
        }

        [TestMethod]
        [Description("HttpParameterDescription Namespace property setter will throw when constructed from MessagePartDescription")]
        public void HttpParameterDescription_Namespace_Immutable_From_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            HttpParameterDescription hpd = new HttpParameterDescription(od.Messages[0].Body.Parts[0]);
            ExceptionAssert.Throws(
                typeof(NotSupportedException),
                "Setting namespace property should throw",
                () => { hpd.Namespace = "newName"; }
                );
        }

        [TestMethod]
        [Description("HttpParameterDescription Index property setter updates MessagePartDescription")]
        public void HttpParameterDescription_Index_Property_Updates_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            MessagePartDescription mpd = od.Messages[0].Body.Parts[0];
            HttpParameterDescription hpd = new HttpParameterDescription(mpd);
            hpd.Index = 5;
            Assert.AreEqual(5, mpd.Index, "Setting index on http parameter description should update messagePartDescription");
            Assert.AreEqual(5, hpd.Index, "Setting index did not retain value");
        }

        [TestMethod]
        [Description("HttpParameterDescription ProcessorType property setter updates MessagePartDescription")]
        public void HttpParameterDescription_Type_Property_Updates_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            MessagePartDescription mpd = od.Messages[0].Body.Parts[0];
            HttpParameterDescription hpd = new HttpParameterDescription(mpd);
            hpd.ParameterType = typeof(float);
            Assert.AreEqual(typeof(float), mpd.Type, "Setting type on http parameter description should update messagePartDescription");
            Assert.AreEqual(typeof(float), hpd.ParameterType, "Setting type did not retain value");
        }

        #endregion Update HttpParameterDescription Tests

        #region Update MessagePartDescription Tests

        [TestMethod]
        [Description("HttpParameterDescription Index property getter updated from MessagePartDescription")]
        public void HttpParameterDescription_Index_Property_Updated_From_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            MessagePartDescription mpd = od.Messages[0].Body.Parts[0];
            HttpParameterDescription hpd = new HttpParameterDescription(mpd);
            mpd.Index = 5;
            Assert.AreEqual(5, hpd.Index, "Setting index on messagePartDescription should update http parameter description");
        }

        [TestMethod]
        [Description("HttpParameterDescription ProcessorType property getter updated from MessagePartDescription")]
        public void HttpParameterDescription_Type_Property_Updated_From_MessagePartDescription()
        {
            OperationDescription od = GetOperationDescription(typeof(MockService3), "SampleInOutMethod");
            MessagePartDescription mpd = od.Messages[0].Body.Parts[0];
            HttpParameterDescription hpd = new HttpParameterDescription(mpd);
            mpd.Type = typeof(float);
            Assert.AreEqual(typeof(float), hpd.ParameterType, "Setting type on messagePartDescription should update http parameter description");
        }

        #endregion Update MessagePartDescription

        #region Test helpers

        public static OperationDescription GetOperationDescription(Type contractType, string methodName)
        {
            ContractDescription cd = ContractDescription.GetContract(contractType);
            OperationDescription od = cd.Operations.FirstOrDefault(o => o.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(od, "Failed to get operation description for " + methodName);
            return od;
        }
        #endregion Test helpers
    }
}
