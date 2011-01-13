using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ServiceModel.Description
{
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public interface IServiceDescriptionFactory
    {
        ServiceDescription CreateDescription(Type serviceType, IDictionary<string, ContractDescription> implementedContracts, ServiceHost host);
    }
}
