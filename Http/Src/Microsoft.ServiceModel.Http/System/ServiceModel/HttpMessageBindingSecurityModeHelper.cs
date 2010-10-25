// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel
{
    internal static class HttpMessageBindingSecurityModeHelper
    {
        internal static bool IsDefined(HttpMessageBindingSecurityMode value)
        {
            return value == HttpMessageBindingSecurityMode.None ||
                value == HttpMessageBindingSecurityMode.Transport ||
                value == HttpMessageBindingSecurityMode.TransportCredentialOnly;
        }
    }
}
