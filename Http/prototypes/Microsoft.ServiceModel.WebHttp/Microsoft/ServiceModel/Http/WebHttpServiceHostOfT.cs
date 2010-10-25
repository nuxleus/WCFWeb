// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;

    public class WebHttpServiceHost<TService> : WebHttpServiceHost
    {
        public WebHttpServiceHost(params Uri[] baseAddresses)
            : this(null, baseAddresses)
        {
        }

        public WebHttpServiceHost(HostConfiguration configuration, params Uri[] baseAddresses)
            : base(typeof(TService), configuration,  baseAddresses)
        {
        }
    }
}