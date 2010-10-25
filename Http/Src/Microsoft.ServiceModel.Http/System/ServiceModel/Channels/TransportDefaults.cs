// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Channels
{
    using System.Security.Principal;

    internal static class TransportDefaults
    {
        internal const bool ExtractGroupsForWindowsAccounts = true;
        internal const TokenImpersonationLevel ImpersonationLevel = TokenImpersonationLevel.Identification;
        internal const bool ManualAddressing = false;
        internal const long MaxBufferPoolSize = 0x80000L;
        internal const int MaxBufferSize = 0x10000;
        internal const int MaxDrainSize = 0x10000;
        internal const int MaxFaultSize = 0x10000;
        internal const long MaxReceivedMessageSize = 0x10000L;
        internal const int MaxRMFaultSize = 0x10000;
        internal const int MaxSecurityFaultSize = 0x4000;
        internal const bool RequireClientCertificate = false;
    }
}
