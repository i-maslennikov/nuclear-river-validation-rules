using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.ValidationRules.Domain.Model.Aggregates;
using NuClear.ValidationRules.Domain.Specifications;

namespace NuClear.ValidationRules.Domain
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class AggregateConstructionMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        private readonly HierarchyMetadata _metadata =
            HierarchyMetadata.Config
                             .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.PriceContextAggregates))
                             .Childs(
                                     AggregateMetadata<Price>.Config
                                                             .HasSource(Specs.Map.Facts.ToAggregates.Prices)
                                                             .HasValueObject(Specs.Map.Facts.ToAggregates.AdvertisementAmountRestrictions, Specs.Find.Aggs.AdvertisementAmountRestrictions)
                                                             .HasValueObject(Specs.Map.Facts.ToAggregates.DeniedPositions,Specs.Find.Aggs.DeniedPositions)
                                                             .HasValueObject(Specs.Map.Facts.ToAggregates.MasterPositions,Specs.Find.Aggs.MasterPositions),

                                     AggregateMetadata<Order>.Config
                                                             .HasSource(Specs.Map.Facts.ToAggregates.Orders)
                                                             .HasValueObject(Specs.Map.Facts.ToAggregates.OrderPositions, Specs.Find.Aggs.OrderPositions)
                                                             .HasValueObject(Specs.Map.Facts.ToAggregates.OrderPrices, Specs.Find.Aggs.OrderPrices),

                                     AggregateMetadata<Position>.Config
                                                             .HasSource(Specs.Map.Facts.ToAggregates.Positions),

                                     AggregateMetadata<Period>.Config
                                                             .HasSource(Specs.Map.Facts.ToAggregates.Periods));

        public AggregateConstructionMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> { { _metadata.Identity.Id, _metadata } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}