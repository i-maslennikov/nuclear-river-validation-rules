using System;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Replication.OperationsProcessing.Metadata;

namespace NuClear.Replication.OperationsProcessing
{
    public sealed class OperationIdentityRegistryFactory
    {
        private readonly IMetadataProvider _metadataProvider;

        public OperationIdentityRegistryFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IOperationIdentityRegistry RegistryFor<TSubDomain>()
            where TSubDomain : ISubDomain
        {
            var metadataId = OperationRegistryMetadataIdentity.Instance.Id.WithRelative(new Uri(typeof(TSubDomain).Name, UriKind.Relative));

            OperationRegistryMetadataElement metadata;
            if (!_metadataProvider.TryGetMetadata(metadataId, out metadata))
            {
                throw new Exception($"Operation identity metadata not found");
            }

            var operationIdentities = metadata.AllowedOperations.Select(x => x.OperationIdentity)
                                              .Concat(metadata.IgnoredOperations.Select(x => x.OperationIdentity))
                                              .Where(x => x.IsNonCoupled())
                                              .Distinct();

            var identityRegistry = new OperationIdentityRegistry(operationIdentities);
            return identityRegistry;
        }
    }
}