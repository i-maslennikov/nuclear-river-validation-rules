using System;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class OperationRegistry<TSubDomain>
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
            return _metadata.AllowedOperationIdentities.Contains(operationIdentity);
        }

        public bool IsDisallowedOperation(StrictOperationIdentity operationIdentity)
        {
            return _metadata.DisallowedOperationIdentities.Contains(operationIdentity);
        }

        public bool TryGetExplicitlyMappedEntityType(IEntityType entityType, out IEntityType mappedEntityType)
        {
            return _metadata.ExplicitEntityTypesMap.TryGetValue(entityType, out mappedEntityType);
        }
    }
}