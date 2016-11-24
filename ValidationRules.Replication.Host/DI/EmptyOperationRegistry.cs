using NuClear.Model.Common.Operations.Identity;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    //todo: Проверь наличие такого в NuClear.Replication.OperationsProcessing.Primary. Если уже есть, удали этот.
    public sealed class EmptyOperationRegistry<T> : IOperationRegistry<T>
    {
        public bool IsAllowedOperation(StrictOperationIdentity operationIdentity)
            => true;

        public bool IsIgnoredOperation(StrictOperationIdentity operationIdentity)
            => false;
    }
}
