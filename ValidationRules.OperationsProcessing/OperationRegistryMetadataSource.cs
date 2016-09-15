using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.OperationsProcessing.Metadata;
using NuClear.ValidationRules.OperationsProcessing.Contexts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class OperationRegistryMetadataSource : MetadataSourceBase<OperationRegistryMetadataIdentity>
    {
        public OperationRegistryMetadataSource()
        {
            var metadataElements = new OperationRegistryMetadataElement[]
                {
                    OperationRegistryMetadataElement
                        .Config
                        .For<AccountFactsSubDomain>(),

                    OperationRegistryMetadataElement
                        .Config
                        .For<ConsistencyFactsSubDomain>(),

                    OperationRegistryMetadataElement
                        .Config
                        .For<PriceFactsSubDomain>(),
                };

            Metadata = metadataElements.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}