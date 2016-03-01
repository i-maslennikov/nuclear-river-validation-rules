using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model;
using NuClear.ValidationRules.Domain.Model;
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
                                     AggregateMetadata<Price, long>
                                         .Config
                                         .HasSource(Specs.Map.Facts.ToAggregates.Prices)
                                         .HasIdentityProvider(DefaultIdentityProvider.Instance)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.AdvertisementAmountRestrictions, Specs.Find.Aggs.AdvertisementAmountRestrictions)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.PriceDeniedPositions, Specs.Find.Aggs.PriceDeniedPositions)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.PriceAssociatedPositions, Specs.Find.Aggs.PriceAssociatedPositions),

                                     AggregateMetadata<Ruleset, long>
                                         .Config
                                         .HasSource(Specs.Map.Facts.ToAggregates.Rulesets)
                                         .HasIdentityProvider(DefaultIdentityProvider.Instance)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.RulesetDeniedPositions, Specs.Find.Aggs.RulesetDeniedPositions)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.RulesetAssociatedPositions, Specs.Find.Aggs.RulesetAssociatedPositions),

                                     AggregateMetadata<Order, long>
                                         .Config
                                         .HasSource(Specs.Map.Facts.ToAggregates.Orders)
                                         .HasIdentityProvider(DefaultIdentityProvider.Instance)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.OrderPositions, Specs.Find.Aggs.OrderPositions)
                                         .HasValueObject(Specs.Map.Facts.ToAggregates.OrderPrices, Specs.Find.Aggs.OrderPrices),

                                     AggregateMetadata<Position, long>
                                         .Config
                                         .HasSource(Specs.Map.Facts.ToAggregates.Positions)
                                         .HasIdentityProvider(DefaultIdentityProvider.Instance),

                                     AggregateMetadata<Period, PeriodId>
                                         .Config
                                         .HasSource(Specs.Map.Facts.ToAggregates.Periods)
                                         .HasIdentityProvider(PeriodIdentityProvider.Instance));

        public AggregateConstructionMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> { { _metadata.Identity.Id, _metadata } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}