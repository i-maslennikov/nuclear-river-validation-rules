using System;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.Replication.OperationsProcessing
{
    public sealed class MetadataOperationIdentityRegistry<TSubDomain> : IOperationIdentityRegistry
        where TSubDomain : ISubDomain
    {
        private readonly IOperationIdentityRegistry _identityRegistry;

        public MetadataOperationIdentityRegistry(IMetadataProvider metadataProvider)
        {
            OperationRegistryMetadataElement metadata;
            var metadataId = OperationRegistryMetadataIdentity.Instance.Id.WithRelative(new Uri(typeof(TSubDomain).Name, UriKind.Relative));
            if (!metadataProvider.TryGetMetadata(metadataId, out metadata))
            {
                throw new Exception($"Operation identity metadata not found");
            }

            var operationIdentities = metadata.AllowedOperations.Select(x => x.OperationIdentity)
                                              .Concat(metadata.IgnoredOperations.Select(x => x.OperationIdentity))
                                              .Where(x => x.IsNonCoupled())
                                              .Distinct();

            _identityRegistry = new OperationIdentityRegistry(operationIdentities);
        }

        public TOperationIdentity GetIdentity<TOperationIdentity>() where TOperationIdentity : IOperationIdentity
            => _identityRegistry.GetIdentity<TOperationIdentity>();

        public IOperationIdentity GetIdentity(Type identityType)
            => _identityRegistry.GetIdentity(identityType);

        public IOperationIdentity GetIdentity(int operationId)
            => _identityRegistry.GetIdentity(operationId);

        public bool TryGetIdentity(int operationId, out IOperationIdentity identity)
            => _identityRegistry.TryGetIdentity(operationId, out identity);

        public IOperationIdentity[] Identities
            => _identityRegistry.Identities;
    }
}