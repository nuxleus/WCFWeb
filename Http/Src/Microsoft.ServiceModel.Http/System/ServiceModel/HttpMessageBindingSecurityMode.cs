// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel
{
    /// <summary>
    /// Defines the modes of security that can be used to configure a service endpoint that uses the
    /// <see cref="HttpMessageBinding"/>.
    /// </summary>
    public enum HttpMessageBindingSecurityMode
    {
        /// <summary>
        /// Indicates no security is used with HTTP requests.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that transport-level security is used with HTTP requests.
        /// </summary>
        Transport,

        /// <summary>
        /// Indicates that only HTTP-based client authentication is provided.
        /// </summary>
        TransportCredentialOnly,
    }
}
