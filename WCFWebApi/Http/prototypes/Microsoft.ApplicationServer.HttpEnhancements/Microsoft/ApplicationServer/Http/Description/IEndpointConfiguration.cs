﻿// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Description
{
    using System.ServiceModel.Description;

    public interface IEndpointConfiguration
    {
        void Configure(ServiceEndpoint endpoint);
    }
}