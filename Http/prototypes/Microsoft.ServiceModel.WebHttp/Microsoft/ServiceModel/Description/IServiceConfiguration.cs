// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Description
{
    using System.ServiceModel.Description;

    public interface IServiceConfiguration
    {
        void Configure(ServiceDescription service);
    }
}