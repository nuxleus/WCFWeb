// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public interface IServiceDescriptionFactory
    {
        ServiceDescription CreateDescription(Type serviceType, IDictionary<string, ContractDescription> implementedContracts, ServiceHost host);
    }
}
