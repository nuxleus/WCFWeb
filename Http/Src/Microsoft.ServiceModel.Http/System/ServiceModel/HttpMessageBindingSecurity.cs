// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel
{
    using System.ComponentModel;

    /// <summary>
    /// Specifies the types of security available to a service endpoint configured to use an
    /// <see cref="HttpMessageBinding"/> binding.
    /// </summary>
    public sealed class HttpMessageBindingSecurity
    {
        internal const HttpMessageBindingSecurityMode DefaultMode = HttpMessageBindingSecurityMode.None;
        
        private HttpMessageBindingSecurityMode mode;
        private HttpTransportSecurity transportSecurity;

        /// <summary>
        /// Creates a new instance of the <see cref="HttpMessageBindingSecurity"/> class.
        /// </summary>
        public HttpMessageBindingSecurity()
        {
            this.mode = DefaultMode;
            this.transportSecurity = new HttpTransportSecurity();
        }

        /// <summary>
        /// Gets or sets the mode of security that is used by an endpoint configured to use an
        /// <see cref="HttpMessageBinding"/> binding.
        /// </summary>
        public HttpMessageBindingSecurityMode Mode
        {
            get 
            { 
                return this.mode; 
            }

            set
            {
                if (!HttpMessageBindingSecurityModeHelper.IsDefined(value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(HttpMessageBindingSecurityMode));
                }

                this.mode = value;
            }
        }

        /// <summary>
        /// Gets or sets an object that contains the transport-level security settings for the 
        /// <see cref="HttpMessageBinding"/> binding.
        /// </summary>
        public HttpTransportSecurity Transport
        {
            get 
            { 
                return this.transportSecurity; 
            }

            set
            {
                this.transportSecurity = value ?? new HttpTransportSecurity();
            }
        }
    }
}
