using System;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Replication.OperationsProcessing.Metadata;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class OperationRegistry<TSubDomain> : IOperationRegistry<TSubDomain>
        where TSubDomain : ISubDomain
    {
        private readonly OperationRegistryMetadataElement _metadata;

        public OperationRegistry(IMetadataProvider metadataProvider)
        {
            var metadataId = OperationRegistryMetadataIdentity.Instance.Id.WithRelative(new Uri(typeof(TSubDomain).Name, UriKind.Relative));
            if (!metadataProvider.TryGetMetadata(metadataId, out _metadata))
            {
                throw new ArgumentException(nameof(metadataProvider));
            }
        }

        public bool IsAllowedOperation(StrictOperationIdentity operationIdentity)
        {
            return _metadata.AllowedOperations.Contains(operationIdentity);
        }

        public bool IsIgnoredOperation(StrictOperationIdentity operationIdentity)
        {
            return _metadata.IgnoredOperations.Contains(operationIdentity);
        }
    }
}