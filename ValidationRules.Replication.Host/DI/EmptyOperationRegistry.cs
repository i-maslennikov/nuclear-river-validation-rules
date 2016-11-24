using NuClear.Model.Common.Operations.Identity;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    public sealed class EmptyOperationRegistry<T> : IOperationRegistry<T>
    {
        public bool IsAllowedOperation(StrictOperationIdentity operationIdentity)
            => true;

        public bool IsIgnoredOperation(StrictOperationIdentity operationIdentity)
            => false;
    }
}
