using System;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class EmptyOperationIdentityRegistry : IOperationIdentityRegistry
    {
        public TOperationIdentity GetIdentity<TOperationIdentity>() where TOperationIdentity : IOperationIdentity
        {
            throw new NotImplementedException();
        }

        public IOperationIdentity GetIdentity(Type identityType)
        {
            throw new NotImplementedException();
        }

        public IOperationIdentity GetIdentity(int operationId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetIdentity(int operationId, out IOperationIdentity identity)
        {
            identity = null;
            return false;
        }

        public IOperationIdentity[] Identities { get; } = Array.Empty<IOperationIdentity>();
    }
}
