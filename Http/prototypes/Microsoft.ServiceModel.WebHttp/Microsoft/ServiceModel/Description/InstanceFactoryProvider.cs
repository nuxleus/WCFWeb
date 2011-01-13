using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ServiceModel.Description
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    public class InstanceFactoryProvider : IInstanceProvider
    {
        private readonly Type serviceType;

        private readonly IInstanceFactory instanceFactory;

        public InstanceFactoryProvider(Type serviceType, IInstanceFactory instanceFactory)
        {
            this.serviceType = serviceType;
            this.instanceFactory = instanceFactory;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return this.instanceFactory.GetInstance(serviceType, instanceContext, message);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            this.instanceFactory.ReleaseInstance(instanceContext, instance);
        }
    }
}
