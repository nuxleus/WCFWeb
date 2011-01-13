namespace Microsoft.ServiceModel.Description
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public interface IInstanceFactory
    {
        object GetInstance(Type serviceType, InstanceContext instanceContext, Message message);

        void ReleaseInstance(InstanceContext instanceContext, object service);
    }
}