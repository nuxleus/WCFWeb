namespace Microsoft.ServiceModel.Description
{
    using System.ServiceModel.Description;

    public interface IContractConfiguration
    {
        void Configure(ContractDescription contract);
    }
}