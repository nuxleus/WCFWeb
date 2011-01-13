namespace Microsoft.ServiceModel.Description
{
    using System.ServiceModel.Description;

    public interface IOperationConfiguration
    {
        void Configure(OperationDescription operation);
    }
}