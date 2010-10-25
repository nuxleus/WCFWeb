// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.ServiceModel;

    /// <summary>
    /// An XML element that configures the security for a service with endpoints that use the
    /// <see cref="System.ServericeModel.HttpMessageBinding">HttpMessageBinding</see>. 
    /// This class cannot be inherited.
    /// </summary>
    public sealed class HttpMessageBindingSecurityElement : ConfigurationElement
    {
        private const string ModeString = "mode";
        private const string TransportString = "transport";
        private ConfigurationPropertyCollection properties;

        /// <summary>
        /// Gets or sets an XML element that specifies the security mode for a basic HTTP service.
        /// </summary>
        [ConfigurationProperty(ModeString, DefaultValue = HttpMessageBindingSecurity.DefaultMode)]
        [InternalEnumValidator(typeof(HttpMessageBindingSecurityModeHelper))]
        public HttpMessageBindingSecurityMode Mode
        {
            get { return (HttpMessageBindingSecurityMode)base[ModeString]; }
            set { base[ModeString] = value; }
        }
        
        /// <summary>
        /// Gets an XML element that indicates the transport-level security settings 
        /// for a service endpoint configured to receive HTTP requests.
        /// </summary>
        [ConfigurationProperty(TransportString)]
        public HttpTransportSecurityElement Transport
        {
            get { return (HttpTransportSecurityElement)base[TransportString]; }
        }

        /// <summary>
        /// Gets the collection of properties. (Inherited from 
        /// <see cref="System.Configuration.ConfigurationElement">ConfigurationElement</see>.)
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection localProperties = new ConfigurationPropertyCollection();
                    localProperties.Add(new ConfigurationProperty(ModeString, typeof(HttpMessageBindingSecurityMode), HttpMessageBindingSecurity.DefaultMode, null, new InternalEnumValidator(typeof(HttpMessageBindingSecurityModeHelper)), ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(TransportString, typeof(HttpTransportSecurityElement), null, null, null, ConfigurationPropertyOptions.None));
                    this.properties = localProperties;
                }

                return this.properties;
            }
        }

        internal static void InitializeFromTransport(HttpTransportSecurityElement element, HttpTransportSecurity security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            element.ClientCredentialType = security.ClientCredentialType;
            element.ProxyCredentialType = security.ProxyCredentialType;
            element.Realm = security.Realm;
        }

        internal void ApplyConfiguration(HttpMessageBindingSecurity security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (this.ElementInformation.Properties[ModeString].IsModified)
            {
                security.Mode = this.Mode;
                ApplyConfigurationToTransport(this.Transport, security.Transport);
            }
        }

        internal void InitializeFrom(HttpMessageBindingSecurity security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (security.Mode != HttpMessageBindingSecurity.DefaultMode)
            {
                this.Mode = security.Mode;
            }

            InitializeFromTransport(this.Transport, security.Transport);
        }

        private static void ApplyConfigurationToTransport(HttpTransportSecurityElement element, HttpTransportSecurity security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            security.ClientCredentialType = element.ClientCredentialType;
            security.ProxyCredentialType = element.ProxyCredentialType;
            security.Realm = element.Realm;
        }
    }
}
