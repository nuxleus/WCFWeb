// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageBindingSecurityTests
    {
        #region Type Tests

        [TestMethod]
        [Description("HttpMessageBindingSecurity is a public, non-abstract class.")]
        public void HttpMessageBindingSecurity_Is_A_Public_Non_Abstract_Class()
        {
            Type type = typeof(HttpMessageBindingSecurity);
            Assert.IsTrue(type.IsPublic, "HttpMessageBindingSecurity should be public.");
            Assert.IsTrue(type.IsVisible, "HttpMessageBindingSecurity should be visible.");
            Assert.IsFalse(type.IsAbstract, "HttpMessageBindingSecurity should not be abstract.");
            Assert.IsTrue(type.IsClass, "HttpMessageBindingSecurity should be a class.");
        }

        #endregion Type Tests

        #region Property Tests

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Mode is HttpMessageBindingSecurityMode.None by default.")]
        public void Mode_Is_None_By_Default()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            Assert.AreEqual(HttpMessageBindingSecurityMode.None, security.Mode, "HttpMessageBindingSecurity.Mode should have been HttpMessageBindingSecurityMode.None by default.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Mode can be set.")]
        public void Mode_Can_Be_Set()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            security.Mode = HttpMessageBindingSecurityMode.Transport;
            Assert.AreEqual(HttpMessageBindingSecurityMode.Transport, security.Mode, "HttpMessageBindingSecurity.Mode should have been HttpMessageBindingSecurityMode.Transport.");

            security.Mode = HttpMessageBindingSecurityMode.TransportCredentialOnly;
            Assert.AreEqual(HttpMessageBindingSecurityMode.TransportCredentialOnly, security.Mode, "HttpMessageBindingSecurity.Mode should have been HttpMessageBindingSecurityMode.TransportCredentialOnly.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Mode throws with an invalid HttpMessageBindingSecurityMode value.")]
        public void Mode_Throws_With_Invalid_HttpMessageBindingSecurityMode_Value()
        {
            ExceptionAssert.Throws(
                typeof(System.ComponentModel.InvalidEnumArgumentException),
                "HttpMessageBindingSecurity.Mode should have thrown since HttpMessageBindingSecurityMode is not a valid value.",
                () =>
                {
                    HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
                    security.Mode = (HttpMessageBindingSecurityMode)99;
                });
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport is not null.")]
        public void Transport_Is_Not_Null()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            Assert.IsNotNull(security.Transport, "HttpMessageBindingSecurity.Transport should not be null.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport can be set.")]
        public void Transport_Can_Be_Set()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            HttpTransportSecurity transport = new HttpTransportSecurity();
            transport.ClientCredentialType = HttpClientCredentialType.Basic;
            security.Transport = transport;
            Assert.AreEqual(HttpClientCredentialType.Basic, security.Transport.ClientCredentialType, "HttpMessageBindingSecurity.Transport.ClientCredentialType should have been HttpClientCredentialType.Basic.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport is reset to a new HttpTransportSecurity instance if Transport is set to null.")]
        public void Transport_Resets_To_New_Instance_If_Set_To_Null()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            HttpTransportSecurity transport = security.Transport;
            transport.ClientCredentialType = HttpClientCredentialType.Basic;
            security.Transport = null;
            Assert.AreNotSame(transport, security.Transport, "HttpMessageBindingSecurity.Transport should have been a new instance of HttpTransportSecurity.");
            Assert.AreEqual(HttpClientCredentialType.None, security.Transport.ClientCredentialType, "HttpMessageBindingSecurity.Transport.ClientCredentialType should have been HttpClientCredentialType.None.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport.ClientCredentialType is HttpClientCredentialType.None by default.")]
        public void Transport_ClientCredentialType_Is_None_By_Default()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            Assert.AreEqual(HttpClientCredentialType.None, security.Transport.ClientCredentialType, "HttpMessageBindingSecurity.Transport.ClientCredentialType should have been HttpClientCredentialType.None by default.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport.HttpProxyCredentialType is HttpProxyCredentialType.None by default.")]
        public void Transport_HttpProxyCredentialType_Is_None_By_Default()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            Assert.AreEqual(HttpProxyCredentialType.None, security.Transport.ProxyCredentialType, "HttpMessageBindingSecurity.Transport.ProxyCredentialType should have been HttpProxyCredentialType.None by default.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport.ExtendedProtectionPolicy.PolicyEnforcement is PolicyEnforcement.Never by default.")]
        public void Transport_ExtendedProtectionPolicy_PolicyEnforcement_Is_Never_By_Default()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            Assert.AreEqual(PolicyEnforcement.Never, security.Transport.ExtendedProtectionPolicy.PolicyEnforcement, "HttpMessageBindingSecurity.Transport.ExtendedProtectionPolicy.PolicyEnforcement should have been PolicyEnforcement.Never by default.");
        }

        [TestMethod]
        [Description("HttpMessageBindingSecurity.Transport.Realm is empty string by default.")]
        public void Transport_Realm_Is_Empty_String_By_Default()
        {
            HttpMessageBindingSecurity security = new HttpMessageBindingSecurity();
            Assert.AreEqual(string.Empty, security.Transport.Realm, "HttpMessageBindingSecurity.Transport.Realm should have been empty string.");
        }

        #endregion Property Tests
    }
}
