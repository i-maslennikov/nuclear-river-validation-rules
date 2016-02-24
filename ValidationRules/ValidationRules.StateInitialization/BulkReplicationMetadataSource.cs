using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.Metadata;
using NuClear.River.Common.Metadata.Identities;
using NuClear.ValidationRules.Domain;
using NuClear.ValidationRules.Domain.Dto;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization
{
    public sealed class BulkReplicationMetadataSource : MetadataSourceBase<BulkReplicationMetadataKindIdentity>
    {
        private static readonly IReadOnlyDictionary<Uri, IMetadataElement> Elements =
            new BulkReplicationMetadataElement[]
            {
                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-facts")
                                              .From(ErmConnectionStringIdentity.Instance, Schema.Erm)
                                              .To(FactsConnectionStringIdentity.Instance, Schema.Facts)
                                              .UsingMetadataOfKind<ReplicationMetadataIdentity>(ReplicationMetadataName.PriceContextFacts),

                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-config")
                                              .From(OrderValidationConfigIdentity.Instance, typeof(OrderValidationConfigParser))
                                              .To(FactsConnectionStringIdentity.Instance, Schema.Facts)
                                              .UsingMetadataOfKind<ImportStatisticsMetadataIdentity>(ReplicationMetadataName.PriceContextConfig),

                BulkReplicationMetadataElement.Config
                                              .CommandlineKey("-aggs")
                                              .From(FactsConnectionStringIdentity.Instance, Schema.Facts)
                                              .To(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates)
                                              .UsingMetadataOfKind<ReplicationMetadataIdentity>(ReplicationMetadataName.PriceContextAggregates),

            }.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata => Elements;
    }
}