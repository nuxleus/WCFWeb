// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// A configuration element for the <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see>
    /// binding. 
    /// </summary>
    public class HttpMessageBindingElement : StandardBindingElement
    {
        private const string HostNameComparisonModeString = "hostNameComparisonMode";
        private const string MaxBufferPoolSizeString = "maxBufferPoolSize";
        private const string MaxBufferSizeString = "maxBufferSize";
        private const string MaxReceivedMessageSizeString = "maxReceivedMessageSize";
        private const string SecurityString = "security";
        private const string TransferModeString = "transferMode";
        private ConfigurationPropertyCollection properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBindingElement"/> class and specifies 
        /// the name of the element. 
        /// </summary>
        /// <param name="name">The name that is used for this binding configuration element</param>
        public HttpMessageBindingElement(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBindingElement"/> class.
        /// </summary>
        public HttpMessageBindingElement()
            : this(null)
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the hostname is used to reach the service when matching 
        /// the URI.
        /// </summary>
        [ConfigurationProperty(HostNameComparisonModeString, DefaultValue = HttpTransportDefaults.HostNameComparisonModeDefault)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get { return (HostNameComparisonMode)base[HostNameComparisonModeString]; }
            set { base[HostNameComparisonModeString] = value; }
        }

        /// <summary>
        /// Gets or sets the maximum amount of memory allocated for the buffer manager that manages the buffers 
        /// required by endpoints that use this binding.
        /// </summary>
        [ConfigurationProperty(MaxBufferPoolSizeString, DefaultValue = TransportDefaults.MaxBufferPoolSize)]
        [LongValidator(MinValue = 0)]
        public long MaxBufferPoolSize
        {
            get { return (long)base[MaxBufferPoolSizeString]; }
            set { base[MaxBufferPoolSizeString] = value; }
        }

        /// <summary>
        /// Gets or sets the maximum amount of memory that is allocated for use by the manager of the message 
        /// buffers that receive messages from the channel.
        /// </summary>
        [ConfigurationProperty(MaxBufferSizeString, DefaultValue = TransportDefaults.MaxBufferSize)]
        [IntegerValidator(MinValue = 1)]
        public int MaxBufferSize
        {
            get { return (int)base[MaxBufferSizeString]; }
            set { base[MaxBufferSizeString] = value; }
        }

        /// <summary>
        /// Gets or sets the maximum size for a message that can be processed by the binding.
        /// </summary>
        [ConfigurationProperty(MaxReceivedMessageSizeString, DefaultValue = TransportDefaults.MaxReceivedMessageSize)]
        [LongValidator(MinValue = 1)]
        public long MaxReceivedMessageSize
        {
            get { return (long)base[MaxReceivedMessageSizeString]; }
            set { base[MaxReceivedMessageSizeString] = value; }
        }

        /// <summary>
        /// Gets the configuration element that contains the security settings used with this binding.
        /// </summary>
        [ConfigurationProperty(SecurityString)]
        public HttpMessageBindingSecurityElement Security
        {
            get { return (HttpMessageBindingSecurityElement)base[SecurityString]; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the service configured with the binding uses streamed or 
        /// buffered (or both) modes of message transfer.
        /// </summary>
        [ConfigurationProperty(TransferModeString, DefaultValue = HttpTransportDefaults.TransferModeDefault)]
        public TransferMode TransferMode
        {
            get { return (TransferMode)base[TransferModeString]; }
            set { base[TransferModeString] = value; }
        }

        /// <summary>
        /// Gets the <see cref="Syste.Type">Type</see> of binding that this configuration element represents. 
        /// (Overrides <see cref="System.ServiceModel.Configuration.StandardBindingElement.BindingElementType">
        /// StandardBindingElement.BindingElementType</see>.)
        /// </summary>
        protected override Type BindingElementType
        {
            get 
            { 
                return typeof(HttpMessageBinding); 
            }
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
                    ConfigurationPropertyCollection localProperties = base.Properties;
                    localProperties.Add(new ConfigurationProperty(HostNameComparisonModeString, typeof(HostNameComparisonMode), HostNameComparisonMode.StrongWildcard, null, null, ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(MaxBufferSizeString, typeof(int), 65536, null, new IntegerValidator(1, 2147483647, false), ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(MaxBufferPoolSizeString, typeof(long), (long)524288, null, new LongValidator(0, 9223372036854775807, false), ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(MaxReceivedMessageSizeString, typeof(long), (long)65536, null, new LongValidator(1, 9223372036854775807, false), ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(SecurityString, typeof(HttpMessageBindingSecurityElement), null, null, null, ConfigurationPropertyOptions.None));
                    localProperties.Add(new ConfigurationProperty(TransferModeString, typeof(TransferMode), TransferMode.Buffered, null, null, ConfigurationPropertyOptions.None));
                    this.properties = localProperties;
                }

                return this.properties;
            }
        }

        /// <summary>
        /// Initializes the <see cref="HttpMessageBindingElement"/> from an 
        /// <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> instance.
        /// </summary>
        /// <param name="binding">
        /// The <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> instance from which
        /// the <see cref="HttpMessageBindingElement"/> will be initialized.
        /// </param>
        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            HttpMessageBinding httpMessageBinding = (HttpMessageBinding)binding;

            if (httpMessageBinding.HostNameComparisonMode != HttpTransportDefaults.HostNameComparisonModeDefault)
            {
                this.HostNameComparisonMode = httpMessageBinding.HostNameComparisonMode;
            }

            if (httpMessageBinding.MaxBufferSize != TransportDefaults.MaxBufferSize)
            {
                this.MaxBufferSize = httpMessageBinding.MaxBufferSize;
            }

            if (httpMessageBinding.MaxBufferPoolSize != TransportDefaults.MaxBufferPoolSize)
            {
                this.MaxBufferPoolSize = httpMessageBinding.MaxBufferPoolSize;
            }

            if (httpMessageBinding.MaxReceivedMessageSize != TransportDefaults.MaxReceivedMessageSize)
            {
                this.MaxReceivedMessageSize = httpMessageBinding.MaxReceivedMessageSize;
            }

            if (httpMessageBinding.TransferMode != HttpTransportDefaults.TransferModeDefault)
            {
                this.TransferMode = httpMessageBinding.TransferMode;
            }

            this.Security.InitializeFrom(httpMessageBinding.Security);
        }

        /// <summary>
        /// Applies the configuration of the the <see cref="HttpMessageBindingElement"/> to the given
        /// <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> instance.
        /// </summary>
        /// <param name="binding">The <see cref="System.ServiceModel.HttpMessageBinding">HttpMessageBinding</see> 
        /// instance to which the <see cref="HttpMessageBindingElement"/> configuration will be applied.
        /// </param>
        protected override void OnApplyConfiguration(Binding binding)
        {
            HttpMessageBinding httpMessageBinding = (HttpMessageBinding)binding;

            httpMessageBinding.HostNameComparisonMode = this.HostNameComparisonMode;
            httpMessageBinding.MaxBufferPoolSize = this.MaxBufferPoolSize;
            httpMessageBinding.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            httpMessageBinding.TransferMode = this.TransferMode;
            
            PropertyInformationCollection propertyInfo = this.ElementInformation.Properties;
            if (propertyInfo[MaxBufferSizeString].ValueOrigin != PropertyValueOrigin.Default)
            {
                httpMessageBinding.MaxBufferSize = this.MaxBufferSize;
            }

            this.Security.ApplyConfiguration(httpMessageBinding.Security);
        }
    }
}
