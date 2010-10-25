// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http.Test.UnitTests
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Http.Test.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpMessageBindingTests
    {
        #region Type Tests

        [TestMethod]
        [Description("HttpMessageBinding is a public, non-abstract class.")]
        public void HttpMessageBinding_Is_A_Public_Non_Abstract_Class()
        {
            Type type = typeof(HttpMessageBinding);
            Assert.IsTrue(type.IsPublic, "HttpMessageBinding should be public.");
            Assert.IsTrue(type.IsVisible, "HttpMessageBinding should be visible.");
            Assert.IsFalse(type.IsAbstract, "HttpMessageBinding should not be abstract.");
            Assert.IsTrue(type.IsClass, "HttpMessageBinding should be a class.");
        }

        #endregion Type Tests

        #region Constructor Tests

        [TestMethod]
        [Description("The HttpMessageBinding constructor throws with a null configurationName parameter.")]
        public void HttpMessageBinding_Throws_With_Null_ConfigurationName_Parameter()
        {
            ExceptionAssert.ThrowsArgumentNull(
                "The HttpMessageBinding should have thrown since the configurationName parameter was null.",
                "configurationName",
                () =>
                {
                    HttpMessageBinding binding = new HttpMessageBinding((string)null);
                });
        }

        [TestMethod]
        [Description("The HttpMessageBinding constructor throws with an invalid HttpMessageBindingSecurityMode parameter.")]
        public void HttpMessageBinding_Throws_With_Invalid_HttpMessageBindingSecurityMode_Parameter()
        {
            ExceptionAssert.Throws(
                typeof(System.ComponentModel.InvalidEnumArgumentException),
                "The HttpMessageBinding should have thrown since HttpMessageBindingSecurityMode is not a valid value.",
                () =>
                {
                    HttpMessageBinding binding = new HttpMessageBinding((HttpMessageBindingSecurityMode)99);
                });
        }

        #endregion Constructor Tests

        #region Property Tests

        [TestMethod]
        [Description("HttpMessageBinding.Security.Mode is HttpMessageBindingSecurityMode.None by default.")]
        public void Security_Mode_Is_None_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(HttpMessageBindingSecurityMode.None, binding.Security.Mode, "The HttpMessageBinding.Security.Mode should have been HttpMessageBindingSecurityMode.None by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Security.Mode should agree with the value set via the constructor.")]
        public void Security_Mode_Agrees_With_Constructor_Parameter_Value()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            Assert.AreEqual(HttpMessageBindingSecurityMode.Transport, binding.Security.Mode, "The HttpMessageBinding.Security.Mode should have been HttpMessageBindingSecurityMode.Transport.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.EnvelopeVersion is always be EnvelopeVersion.None.")]
        public void EnvelopeVersion_Is_Always_None()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(EnvelopeVersion.None, binding.EnvelopeVersion, "HttpMessageBinding.EnvelopeVersion should always be EnvelopeVersion.None.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.HostNameComparisonMode is HostNameComparisonMode.StrongWildcard by default.")]
        public void HostNameComparisonMode_Is_StrongWildcard_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(HostNameComparisonMode.StrongWildcard, binding.HostNameComparisonMode, "HttpMessageBinding.HostNameComparisonMode should have been HostNameComparisonMode.StrongWildcard by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.HostNameComparisonMode can be set.")]
        public void HostNameComparisonMode_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.HostNameComparisonMode = HostNameComparisonMode.Exact;
            Assert.AreEqual(HostNameComparisonMode.Exact, binding.HostNameComparisonMode, "HttpMessageBinding.HostNameComparisonMode should have been HostNameComparisonMode.Exact.");

            binding.HostNameComparisonMode = HostNameComparisonMode.WeakWildcard;
            Assert.AreEqual(HostNameComparisonMode.WeakWildcard, binding.HostNameComparisonMode, "HttpMessageBinding.HostNameComparisonMode should have been HostNameComparisonMode.WeakWildcard.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.HostNameComparisonMode throws if set to an invalid value.")]
        public void HostNameComparisonMode_Throws_If_Set_To_Invalid_Value()
        {
            HttpMessageBinding binding = new HttpMessageBinding();

            ExceptionAssert.Throws(
                typeof(System.ComponentModel.InvalidEnumArgumentException),
                "The HttpMessageBinding should have thrown since HostNameComparisonMode is not a valid value.",
                () =>
                {
                    binding.HostNameComparisonMode = (HostNameComparisonMode)99;
                });
        }

        [TestMethod]
        [Description("HttpMessageBinding.MaxBufferPoolSize is 0x80000L by default.")]
        public void MaxBufferPoolSize_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(0x80000L, binding.MaxBufferPoolSize, "HttpMessageBinding.MaxBufferPoolSize should have been 0x80000L by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.MaxBufferPoolSize can be set.")]
        public void MaxBufferPoolSize_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.MaxBufferPoolSize = 100;
            Assert.AreEqual(100, binding.MaxBufferPoolSize, "HttpMessageBinding.MaxBufferPoolSize should have been 100.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.MaxBufferSize is 0x10000 by default.")]
        public void MaxBufferSize_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(0x10000, binding.MaxBufferSize, "HttpMessageBinding.MaxBufferSize should have been 0x10000 by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.MaxBufferSize can be set.")]
        public void MaxBufferSize_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.MaxBufferSize = 100;
            Assert.AreEqual(100, binding.MaxBufferSize, "HttpMessageBinding.MaxBufferSize should have been 100.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.MaxReceivedMessageSize is 0x10000L by default.")]
        public void MaxReceivedMessageSize_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(0x10000L, binding.MaxReceivedMessageSize, "HttpMessageBinding.MaxReceivedMessageSize should have been 0x10000L by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.MaxReceivedMessageSize can be set.")]
        public void MaxReceivedMessageSize_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.MaxReceivedMessageSize = 100;
            Assert.AreEqual(100, binding.MaxReceivedMessageSize, "HttpMessageBinding.MaxReceivedMessageSize should have been 100.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.MessageVersion is always be MessageVersion.None.")]
        public void MessageVersion_Is_Always_None()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(MessageVersion.None, binding.MessageVersion, "HttpMessageBinding.MessageVersion should always be MessageVersion.None.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Name is 'HttpMessageBinding' by default.")]
        public void Name_Is_HttpMessageBinding_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual("HttpMessageBinding", binding.Name, "HttpMessageBinding.Name should have been 'HttpMessageBinding'.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Name can be set.")]
        public void Name_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.Name = "SomeName";
            Assert.AreEqual("SomeName", binding.Name, "HttpMessageBinding.Name should have been 'SomeName'.");

            binding.Name = " ";
            Assert.AreEqual(" ", binding.Name, "HttpMessageBinding.Name should have been ' '.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Name to null throws.")]
        public void Setting_Name_To_Null_Throws()
        {
            HttpMessageBinding binding = new HttpMessageBinding();

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "Setting HttpMessageBinding.Name to null should have thrown.",
                () =>
                {                 
                    binding.Name = null;
                });
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Name to null throws.")]
        public void Setting_Name_Empty_String_Throws()
        {
            HttpMessageBinding binding = new HttpMessageBinding();

            ExceptionAssert.Throws(
                typeof(ArgumentException),
                "Setting HttpMessageBinding.Name to empty string should have thrown.",
                () =>
                {
                    binding.Name = string.Empty;
                });
        }

        [TestMethod]
        [Description("HttpMessageBinding.Namespace is 'http://tempuri.org/' by default.")]
        public void Namespace_Is_TempuriOrg_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual("http://tempuri.org/", binding.Namespace, "HttpMessageBinding.Namespace should have been 'http://tempuri.org/' by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Namespace can be set.")]
        public void Namespace_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.Namespace = "http://SomeNamespace.org/";
            Assert.AreEqual("http://SomeNamespace.org/", binding.Namespace, "HttpMessageBinding.Namespace should have been 'http://SomeNamespace.org/'.");

            binding.Namespace = string.Empty;
            Assert.AreEqual(string.Empty, binding.Namespace, "HttpMessageBinding.Namespace should have been an empty string.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Namespace to null throws.")]
        public void Setting_Namespace_To_Null_Throws()
        {
            HttpMessageBinding binding = new HttpMessageBinding();

            ExceptionAssert.ThrowsArgumentNull(
                "Setting HttpMessageBinding.Namespace to null should have thrown.",
                "value",
                () =>
                {
                    binding.Namespace = null;
                });
        }

        [TestMethod]
        [Description("HttpMessageBinding.Scheme is 'http' by default.")]
        public void Scheme_Is_Http_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual("http", binding.Scheme, "HttpMessageBinding.Scheme should have been 'http' by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Scheme is 'https' when the security mode is HttpMessageBindingSecurityMode.Transport.")]
        public void Scheme_Is_Https_With_Transport_Security()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            Assert.AreEqual("https", binding.Scheme, "HttpMessageBinding.Scheme should have been 'https'.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Scheme is 'https' when the security mode is HttpMessageBindingSecurityMode.TransportCredentialOnly.")]
        public void Scheme_Is_Http_With_TransportCredentialOnly_Security()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.TransportCredentialOnly);
            Assert.AreEqual("http", binding.Scheme, "HttpMessageBinding.Scheme should have been 'http'.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.TransferMode is TransferMode.Buffered by default.")]
        public void TransferMode_Is_Buffered_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(TransferMode.Buffered, binding.TransferMode, "HttpMessageBinding.TransferMode should have been TransferMode.Buffered by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.TransferMode can be set.")]
        public void TransferMode_Can_Be_Set()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.TransferMode = TransferMode.Streamed;
            Assert.AreEqual(TransferMode.Streamed, binding.TransferMode, "HttpMessageBinding.TransferMode should have been TransferMode.Streamed.");

            binding.TransferMode = TransferMode.StreamedRequest;
            Assert.AreEqual(TransferMode.StreamedRequest, binding.TransferMode, "HttpMessageBinding.TransferMode should have been TransferMode.StreamedRequest.");

            binding.TransferMode = TransferMode.StreamedResponse;
            Assert.AreEqual(TransferMode.StreamedResponse, binding.TransferMode, "HttpMessageBinding.TransferMode should have been TransferMode.StreamedResponse.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.CloseTimeout is 00:01:00 by default.")]
        public void CloseTimeout_Is_One_Minute_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(new TimeSpan(0, 1, 0), binding.CloseTimeout, "HttpMessageBinding.CloseTimeout should have been 00:01:00 by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.OpenTimeout is 00:01:00 by default.")]
        public void OpenTimeout_Is_One_Minute_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(new TimeSpan(0, 1, 0), binding.OpenTimeout, "HttpMessageBinding.OpenTimeout should have been 00:01:00 by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.ReceiveTimeout is 00:10:00 by default.")]
        public void ReceiveTimeout_Is_Ten_Minutes_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(new TimeSpan(0, 10, 0), binding.ReceiveTimeout, "HttpMessageBinding.ReceiveTimeout should have been 00:10:00 by default.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.Security throws if set to null.")]
        public void Security_Throws_If_Set_To_Null()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.IsNotNull(binding.Security, "HttpMessageBinding.Security should never by null.");

            ExceptionAssert.ThrowsArgumentNull(
                "HttpMessageBinding.Security should have thrown when set to null.",
                "value",
                () =>
                {
                    binding.Security = null;
                });
        }

        #endregion Property Tests

        #region CreateBindingElements Tests

        [TestMethod]
        [Description("HttpMessageBinding.CreateBindingElements.Count is always two regardless of the security mode.")]
        public void CreateBindingElements_Count_Is_Always_Two()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            Assert.AreEqual(2, binding.CreateBindingElements().Count, "The HttpMessageBinding should always have two binding elements.");

            HttpMessageBinding bindingWithSecurity = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            Assert.AreEqual(2, bindingWithSecurity.CreateBindingElements().Count, "The HttpMessageBinding should always have two binding elements.");

            HttpMessageBinding bindingWithSecurity2 = new HttpMessageBinding(HttpMessageBindingSecurityMode.TransportCredentialOnly);
            Assert.AreEqual(2, bindingWithSecurity2.CreateBindingElements().Count, "The HttpMessageBinding should always have two binding elements.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.CreateBindingElements returns HttpTransportBindingElement and HttpMessageEncodingBindingElement binding elements by default.")]
        public void CreateBindingElements_Contains_HttpTransportBindingElement_And_HttpMessageEncodingBindingElement_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.IsNotNull(transport, "The HttpMessageBinding.CreateBindingElements should have returned a collection with HttpTransportBindingElement.");

            HttpMessageEncodingBindingElement encoder = binding.CreateBindingElements().Find<HttpMessageEncodingBindingElement>();
            Assert.IsNotNull(encoder, "The HttpMessageBinding.CreateBindingElements should have returned a collection with HttpMessageEncodingBindingElement.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.CreateBindingElements returns HttpsTransportBindingElement and HttpMessageEncodingBindingElement binding elements with transport security mode.")]
        public void CreateBindingElements_Contains_HttpsTransportBindingElement_And_HttpMessageEncodingBindingElement_With_Transport_Security_Mode()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.IsNotNull(transport, "The HttpMessageBinding.CreateBindingElements should have returned a collection with HttpsTransportBindingElement.");

            HttpMessageEncodingBindingElement encoder = binding.CreateBindingElements().Find<HttpMessageEncodingBindingElement>();
            Assert.IsNotNull(encoder, "The HttpMessageBinding.CreateBindingElements should have returned a collection with HttpMessageEncodingBindingElement.");
        }

        [TestMethod]
        [Description("HttpMessageBinding.CreateBindingElements returns HttpTransportBindingElement and HttpMessageEncodingBindingElement binding elements with credential only security mode.")]
        public void CreateBindingElements_Contains_HttpTransportBindingElement_And_HttpMessageEncodingBindingElement_With_CredentialOnly_Security_Mode()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.TransportCredentialOnly);
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.IsNotNull(transport, "The HttpMessageBinding.CreateBindingElements should have returned a collection with HttpTransportBindingElement.");

            HttpMessageEncodingBindingElement encoder = binding.CreateBindingElements().Find<HttpMessageEncodingBindingElement>();
            Assert.IsNotNull(encoder, "The HttpMessageBinding.CreateBindingElements should have returned a collection with HttpMessageEncodingBindingElement.");
        }

        #endregion CreateBindingElements Tests

        #region HttpTransportBindingElement Tests

        [TestMethod]
        [Description("HttpTransportBindingElement.AuthenticationScheme is AuthenticationSchemes.Anonymous by default.")]
        public void HttpTransportBindingElement_AuthenticationScheme_Is_Anonymous_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(AuthenticationSchemes.Anonymous, transport.AuthenticationScheme, "HttpTransportBindingElement.AuthenticationScheme should have been AuthenticationSchemes.Anonymous by default.");
        }

        [TestMethod]
        [Description("HttpTransportBindingElement.ProxyAuthenticationScheme is AuthenticationSchemes.Anonymous by default.")]
        public void HttpTransportBindingElement_ProxyAuthenticationScheme_Is_Anonymous_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(AuthenticationSchemes.Anonymous, transport.ProxyAuthenticationScheme, "HttpTransportBindingElement.ProxyAuthenticationScheme should have been AuthenticationSchemes.Anonymous by default.");
        }

        [TestMethod]
        [Description("HttpTransportBindingElement.Realm is an empty string by default.")]
        public void HttpTransportBindingElement_Realm_Is_Empty_String_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(string.Empty, transport.Realm, "HttpTransportBindingElement.Realm should have been an empty string by default.");
        }

        [TestMethod]
        [Description("HttpTransportBindingElement.ManualAddressing is 'true' regardless of security mode.")]
        public void HttpTransportBindingElement_ManualAddressing_Is_True()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.IsTrue(transport.ManualAddressing, "HttpTransportBindingElement.ManualAddressing should have been 'true'.");

            binding.Security.Mode = HttpMessageBindingSecurityMode.Transport;
            HttpsTransportBindingElement transport2 = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.IsTrue(transport2.ManualAddressing, "HttpsTransportBindingElement.ManualAddressing should have been 'true'.");

            binding.Security.Mode = HttpMessageBindingSecurityMode.TransportCredentialOnly;
            HttpTransportBindingElement transport3 = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.IsTrue(transport3.ManualAddressing, "HttpTransportBindingElement.ManualAddressing should have been 'true'.");
        }

        [TestMethod]
        [Description("HttpTransportBindingElement.RequireClientCertificate is 'false' by default.")]
        public void HttpsTransportBindingElement_RequireClientCertificate_Is_False_By_Default()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.IsFalse(transport.RequireClientCertificate, "HttpsTransportBindingElement.RequireClientCertificate should have been 'false' by default.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.HostNameComparisonMode configures the HttpTransportBindingElement of the binding.")]
        public void Setting_HostNameComparisonMode_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.HostNameComparisonMode = HostNameComparisonMode.Exact;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(HostNameComparisonMode.Exact, transport.HostNameComparisonMode, "HttpTransportBindingElement.HostNameComparisonMode should have been HostNameComparisonMode.Exact.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.MaxBufferPoolSize configures the HttpTransportBindingElement of the binding.")]
        public void Setting_MaxBufferPoolSize_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.MaxBufferPoolSize = 100;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(100, transport.MaxBufferPoolSize, "HttpTransportBindingElement.MaxBufferPoolSize should have been 100.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.MaxBufferSize configures the HttpTransportBindingElement of the binding.")]
        public void Setting_MaxBufferSize_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.MaxBufferSize = 100;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(100, transport.MaxBufferSize, "HttpTransportBindingElement.MaxBufferSize should have been 100.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.MaxReceivedMessageSize configures the HttpTransportBindingElement of the binding.")]
        public void Setting_MaxReceivedMessageSize_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.MaxReceivedMessageSize = 100;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(100, transport.MaxReceivedMessageSize, "HttpTransportBindingElement.MaxReceivedMessageSize should have been 100.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.TransferMode configures the HttpTransportBindingElement of the binding.")]
        public void Setting_TransferMode_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding();
            binding.TransferMode = TransferMode.StreamedRequest;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(TransferMode.StreamedRequest, transport.TransferMode, "HttpTransportBindingElement.TransferMode should have been TransferMode.StreamedRequest.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.HostNameComparisonMode configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_HostNameComparisonMode_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.HostNameComparisonMode = HostNameComparisonMode.Exact;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(HostNameComparisonMode.Exact, transport.HostNameComparisonMode, "HttpsTransportBindingElement.HostNameComparisonMode should have been HostNameComparisonMode.Exact.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.MaxBufferPoolSize configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_MaxBufferPoolSize_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.MaxBufferPoolSize = 100;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(100, transport.MaxBufferPoolSize, "HttpsTransportBindingElement.MaxBufferPoolSize should have been 100.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.MaxBufferSize configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_MaxBufferSize_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.MaxBufferSize = 100;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(100, transport.MaxBufferSize, "HttpsTransportBindingElement.MaxBufferSize should have been 100.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.MaxReceivedMessageSize configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_MaxReceivedMessageSize_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.MaxReceivedMessageSize = 100;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(100, transport.MaxReceivedMessageSize, "HttpsTransportBindingElement.MaxReceivedMessageSize should have been 100.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.TransferMode configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_TransferMode_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.TransferMode = TransferMode.StreamedRequest;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(TransferMode.StreamedRequest, transport.TransferMode, "HttpsTransportBindingElement.TransferMode should have been TransferMode.StreamedRequest.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.ClientCredentialType configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_ClientCredentialType_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(AuthenticationSchemes.Basic, transport.AuthenticationScheme, "HttpsTransportBindingElement.AuthenticationScheme should have been AuthenticationSchemes.Basic.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.ClientCredentialType configures the HttpTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_ClientCredentialType_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(AuthenticationSchemes.Basic, transport.AuthenticationScheme, "HttpTransportBindingElement.AuthenticationScheme should have been AuthenticationSchemes.Basic.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.ProxyCredentialType configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_ProxyCredentialType_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual(AuthenticationSchemes.Basic, transport.ProxyAuthenticationScheme, "HttpsTransportBindingElement.ProxyAuthenticationScheme should have been AuthenticationSchemes.Basic.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.ProxyCredentialType configures the HttpTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_ProxyCredentialType_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual(AuthenticationSchemes.Basic, transport.ProxyAuthenticationScheme, "HttpTransportBindingElement.ProxyAuthenticationScheme should have been AuthenticationSchemes.Basic.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.Realm configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_Realm_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.Security.Transport.Realm = "SomeRealm";
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.AreEqual("SomeRealm", transport.Realm, "HttpsTransportBindingElement.Realm should have been 'SomeRealm'.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.Realm configures the HttpTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_Realm_Configures_The_HttpTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.Realm = "SomeRealm";
            HttpTransportBindingElement transport = binding.CreateBindingElements().Find<HttpTransportBindingElement>();
            Assert.AreEqual("SomeRealm", transport.Realm, "HttpTransportBindingElement.Realm should have been 'SomeRealm'.");
        }

        [TestMethod]
        [Description("Setting HttpMessageBinding.Security.Transport.ClientCredentialType to HttpClientCredentialType.Certificate configures the HttpsTransportBindingElement of the binding.")]
        public void Setting_Security_Transport_ClientCredentialType_Certificate_Configures_The_HttpsTransportBindingElement()
        {
            HttpMessageBinding binding = new HttpMessageBinding(HttpMessageBindingSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            HttpsTransportBindingElement transport = binding.CreateBindingElements().Find<HttpsTransportBindingElement>();
            Assert.IsTrue(transport.RequireClientCertificate, "HttpsTransportBindingElement.RequireClientCertificate should have been 'true'.");
        }

        #endregion HttpTransportBindingElement Tests

        #region IBindingRuntimePreferences Tests

        [TestMethod]
        [Description("HttpMessageBinding implements IBindingRuntimePreferences.")]
        public void Implements_IBindingRuntimePreferences()
        {
            IBindingRuntimePreferences binding = new HttpMessageBinding() as IBindingRuntimePreferences;
            Assert.IsNotNull(binding, "HttpMessageBinding should implement IBindingRuntimePreferences.");
        }

        [TestMethod]
        [Description("IBindingRuntimePreferences.ReceiveSynchronously always returns 'false'.")]
        public void ReceiveSynchronously_Always_Returns_False()
        {
            IBindingRuntimePreferences binding = new HttpMessageBinding() as IBindingRuntimePreferences;
            Assert.IsFalse(binding.ReceiveSynchronously, "IBindingRuntimePreferences.ReceiveSynchronously should always be 'false'.");
        }

        #endregion IBindingRuntimePreferences Tests
    }
}
