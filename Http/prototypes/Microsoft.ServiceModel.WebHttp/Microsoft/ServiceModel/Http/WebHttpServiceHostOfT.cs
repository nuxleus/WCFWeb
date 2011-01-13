// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Http
{
    using System;

    using Microsoft.ServiceModel.Description;

    public class WebHttpServiceHost<TService> : WebHttpServiceHost
    {
        public WebHttpServiceHost(params Uri[] baseAddresses)
            : this(null, baseAddresses)
        {
        }

        public WebHttpServiceHost(HttpHostConfiguration configuration, params Uri[] baseAddresses)
            : base(typeof(TService), configuration,  baseAddresses)
        {
        }
    }
}