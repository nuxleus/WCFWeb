namespace Microsoft.ServiceModel.Description
{
    using System.ServiceModel.Description;

    public interface IEndpointConfiguration
    {
        void Configure(ServiceEndpoint endpoint);
    }
}