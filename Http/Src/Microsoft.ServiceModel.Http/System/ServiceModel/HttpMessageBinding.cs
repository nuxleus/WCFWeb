// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel
{
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// A binding used to configure endpoints for web services that use strongly-type HTTP request 
    /// and response messages.
    /// </summary>
    public class HttpMessageBinding : Binding, IBindingRuntimePreferences
    {
        internal const string CollectionElementName = "httpMessageBinding";

        private HttpsTransportBindingElement httpsTransportBindingElement;
        private HttpTransportBindingElement httpTransportBindingElement;
        private HttpMessageBindingSecurity security;
        private HttpMessageEncodingBindingElement httpMessageEncodingBindingElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBinding"/> class.
        /// </summary>
        public HttpMessageBinding() : base()
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBinding"/> class with a 
        /// binding specified by its configuration name.
        /// </summary>
        /// <param name="configurationName">
        /// The binding configuration name for the 
        /// <see cref="System.ServiceModel.Configuration.HttpMessageBindingElement">HttpMessageBindingElement</see>.
        /// </param>
        public HttpMessageBinding(string configurationName) : this()
        {
            if (configurationName == null)
            {
                throw new ArgumentNullException("configurationName");
            }

            this.ApplyConfiguration(configurationName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBinding"/> class with the 
        /// type of security used by the binding explicitly specified.
        /// </summary>
        /// <param name="securityMode">The value of <see cref="HttpMessageBindingSecurityMode"/> that 
        /// specifies the type of security that is used to configure a service endpoint using the
        /// <see cref="HttpMessageBinding"/> binding.
        /// </param>
        public HttpMessageBinding(HttpMessageBindingSecurityMode securityMode) : this()
        {
            this.security.Mode = securityMode;
        }

        /// <summary>
        /// Gets the envelope version that is used by endpoints that are configured to use an 
        /// <see cref="HttpMessageBinding"/> binding.  Always returns <see cref="EnvelopeVersion.None"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "The web binding always use EnvelopeVersion.None. This is by design.")]
        public EnvelopeVersion EnvelopeVersion
        {
            get
            {
                return EnvelopeVersion.None;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the hostname is used to reach the 
        /// service when matching the URI.
        /// </summary>
        [DefaultValue(HttpTransportDefaults.HostNameComparisonModeDefault)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get 
            { 
                return this.httpTransportBindingElement.HostNameComparisonMode; 
            }

            set
            {
                this.httpTransportBindingElement.HostNameComparisonMode = value;
                this.httpsTransportBindingElement.HostNameComparisonMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum amount of memory allocated for the buffer manager that manages the buffers 
        /// required by endpoints that use this binding.
        /// </summary>
        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get 
            { 
                return this.httpTransportBindingElement.MaxBufferPoolSize; 
            }

            set
            {
                this.httpTransportBindingElement.MaxBufferPoolSize = value;
                this.httpsTransportBindingElement.MaxBufferPoolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum amount of memory that is allocated for use by the manager of the message 
        /// buffers that receive messages from the channel.
        /// </summary>
        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get 
            { 
                return this.httpTransportBindingElement.MaxBufferSize; 
            }

            set
            {
                this.httpTransportBindingElement.MaxBufferSize = value;
                this.httpsTransportBindingElement.MaxBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum size for a message that can be processed by the binding.
        /// </summary>
        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get 
            { 
                return this.httpTransportBindingElement.MaxReceivedMessageSize; 
            }

            set
            {
                this.httpTransportBindingElement.MaxReceivedMessageSize = value;
                this.httpsTransportBindingElement.MaxReceivedMessageSize = value;
            }
        }

        /// <summary>
        /// Gets the URI transport scheme for the channels and listeners that are configured 
        /// with this binding. (Overrides <see cref="System.ServiceModel.Channels.Binding.Scheme">
        /// Binding.Scheme</see>.)
        /// </summary>
        public override string Scheme
        { 
            get 
            { 
                return this.GetTransport().Scheme; 
            } 
        }

        /// <summary>
        /// Gets or sets the security settings used with this binding. 
        /// </summary>
        public HttpMessageBindingSecurity Security
        {
            get 
            { 
                return this.security; 
            }
            
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.security = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the service configured with the 
        /// binding uses streamed or buffered (or both) modes of message transfer.
        /// </summary>
        [DefaultValue(HttpTransportDefaults.TransferModeDefault)]
        public TransferMode TransferMode
        {
            get 
            { 
                return this.httpTransportBindingElement.TransferMode; 
            }
            
            set
            {
                this.httpTransportBindingElement.TransferMode = value;
                this.httpsTransportBindingElement.TransferMode = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is the pattern used by all standard bindings.")]
        bool IBindingRuntimePreferences.ReceiveSynchronously
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an ordered collection of binding elements contained in the current binding. 
        /// (Overrides <see cref="System.ServiceModel.Channels.Binding.CreateBindingElements">
        /// Binding.CreateBindingElements</see>.)
        /// </summary>
        /// <returns>
        /// An ordered collection of binding elements contained in the current binding.
        /// </returns>
        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection bindingElements = new BindingElementCollection();

            bindingElements.Add(this.httpMessageEncodingBindingElement);
            bindingElements.Add(this.GetTransport());

            return bindingElements.Clone();
        }

        private void ApplyConfiguration(string configurationName)
        {
            HttpMessageBindingCollectionElement section = HttpMessageBindingCollectionElement.GetBindingCollectionElement();

            HttpMessageBindingElement element = null;
            if (section != null && section.Bindings.ContainsKey(configurationName))
            {
                element = section.Bindings[configurationName];
            }

            if (element == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ConfigInvalidBindingConfigurationName,
                        configurationName,
                        CollectionElementName));
            }

            element.ApplyConfiguration(this);
        }

        private TransportBindingElement GetTransport()
        {
            if (this.security.Mode == HttpMessageBindingSecurityMode.Transport)
            {
                this.ConfigureHttpsTransportSecurity();
                return this.httpsTransportBindingElement;
            }
            else if (this.security.Mode == HttpMessageBindingSecurityMode.TransportCredentialOnly)
            {
                this.ConfigureHttpTransportSecurityCredentialOnly();
                return this.httpTransportBindingElement;
            }

            this.httpTransportBindingElement.AuthenticationScheme = AuthenticationSchemes.Anonymous;
            this.httpTransportBindingElement.ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
            this.httpTransportBindingElement.Realm = string.Empty;

            return this.httpTransportBindingElement;
        }

        private void ConfigureHttpTransportSecurityCredentialOnly()
        {
            if (this.security.Transport.ClientCredentialType == HttpClientCredentialType.Certificate)
            {
                throw new InvalidOperationException(SR.CertificateUnsupportedForHttpTransportCredentialOnly);
            }

            this.httpTransportBindingElement.AuthenticationScheme =
                HttpClientCredentialTypeHelper.MapToAuthenticationScheme(this.security.Transport.ClientCredentialType);
            this.httpTransportBindingElement.ProxyAuthenticationScheme =
                HttpProxyCredentialTypeHelper.MapToAuthenticationScheme(this.security.Transport.ProxyCredentialType);
            this.httpTransportBindingElement.Realm = this.security.Transport.Realm;
        }

        private void ConfigureHttpsTransportSecurity()
        {
            this.httpsTransportBindingElement.AuthenticationScheme =
                HttpClientCredentialTypeHelper.MapToAuthenticationScheme(this.security.Transport.ClientCredentialType);
            this.httpsTransportBindingElement.ProxyAuthenticationScheme =
                HttpProxyCredentialTypeHelper.MapToAuthenticationScheme(this.security.Transport.ProxyCredentialType);
            this.httpsTransportBindingElement.Realm = this.security.Transport.Realm;
            this.httpsTransportBindingElement.RequireClientCertificate =
                this.security.Transport.ClientCredentialType == HttpClientCredentialType.Certificate;
        }

        private void Initialize()
        {
            this.security = new HttpMessageBindingSecurity();
            
            this.httpTransportBindingElement = new HttpTransportBindingElement();
            this.httpTransportBindingElement.ManualAddressing = true;
            
            this.httpsTransportBindingElement = new HttpsTransportBindingElement();
            this.httpsTransportBindingElement.ManualAddressing = true;

            this.httpMessageEncodingBindingElement = new HttpMessageEncodingBindingElement();
        }
    }
}
