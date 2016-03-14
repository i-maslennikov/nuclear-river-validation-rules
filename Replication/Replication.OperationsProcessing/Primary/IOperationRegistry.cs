using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    // ReSharper disable once UnusedTypeParameter
    public interface IOperationRegistry<TSubDomain>
    {
        bool IsAllowedOperation(StrictOperationIdentity operationIdentity);
        bool IsIgnoredOperation(StrictOperationIdentity operationIdentity);
    }
}