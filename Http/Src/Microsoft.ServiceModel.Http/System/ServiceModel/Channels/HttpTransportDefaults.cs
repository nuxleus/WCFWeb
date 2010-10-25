// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;

    internal static class HttpTransportDefaults
    {
        internal const bool AllowCookies = false;
        internal const AuthenticationSchemes AuthenticationScheme = AuthenticationSchemes.Anonymous;
        internal const bool BypassProxyOnLocal = false;
        internal const bool DecompressionEnabled = true;
        internal const HostNameComparisonMode HostNameComparisonModeDefault = HostNameComparisonMode.StrongWildcard;
        internal const bool KeepAliveEnabled = true;
        internal const Uri ProxyAddress = null;
        internal const AuthenticationSchemes ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
        internal const string Realm = "";
        internal const TransferMode TransferModeDefault = TransferMode.Buffered;
        internal const bool UnsafeConnectionNtlmAuthentication = false;
        internal const bool UseDefaultWebProxy = true;
    }
}
