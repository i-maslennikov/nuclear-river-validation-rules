using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    // ReSharper disable once UnusedTypeParameter
    public interface IOperationRegistry<TSubDomain>
    {
        bool IsAllowedOperation(StrictOperationIdentity operationIdentity);
        bool IsDisallowedOperation(StrictOperationIdentity operationIdentity);

        bool TryGetExplicitlyMappedEntityType(IEntityType entityType, out IEntityType mappedEntityType);
    }
}