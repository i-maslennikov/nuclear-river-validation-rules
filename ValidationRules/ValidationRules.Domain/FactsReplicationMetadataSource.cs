using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.ValidationRules.Domain.Model.Facts;
using NuClear.ValidationRules.Domain.Specifications;

namespace NuClear.ValidationRules.Domain
{
    using Aggregates = Model.Aggregates;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class FactsReplicationMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        public FactsReplicationMetadataSource()
        {
            HierarchyMetadata factsReplicationMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.PriceContextFacts))
                    .Childs(FactMetadata<AssociatedPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.AssociatedPosition)
                                .HasDependentAggregate<Aggregates::Price>(Specs.Map.Facts.ToPriceAggregate.ByAssociatedPosition)
                                .HasDependentAggregate<Aggregates::Position>(Specs.Map.Facts.ToPositionAggregate.ByAssociatedPosition),

                            FactMetadata<AssociatedPositionsGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.AssociatedPositionsGroup)
                                .HasDependentAggregate<Aggregates::Price>(Specs.Map.Facts.ToPriceAggregate.ByAssociatedPositionGroup),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Category)
                                .HasDependentAggregate<Aggregates::Order>(Specs.Map.Facts.ToOrderAggregate.ByCategory),

                            FactMetadata<DeniedPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.DeniedPosition),

                            // TODO: что с GlobalAssociatedPositions и GlobalDeniedPositions ? по-хорошему надо в Price
                            // дождаться выхода задачи ERM-8801 в ERM

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Order)
                                .HasMatchedAggregate<Aggregates::Order>(),

                            FactMetadata<OrderPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrderPosition)
                                .HasDependentAggregate<Aggregates::Order>(Specs.Map.Facts.ToOrderAggregate.ByOrderPosition),

                            FactMetadata<OrderPositionAdvertisement>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrderPositionAdvertisement)
                                .HasDependentAggregate<Aggregates::Order>(Specs.Map.Facts.ToOrderAggregate.ByOrderPositionAdvertisement),

                            // TODO: period
                            FactMetadata<OrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrganizationUnit),

                            FactMetadata<Position>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Position)
                                .HasMatchedAggregate<Aggregates::Position>()
                                .HasDependentAggregate<Aggregates::Order>(Specs.Map.Facts.ToOrderAggregate.ByPosition),

                            FactMetadata<Price>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Price)
                                .HasMatchedAggregate<Aggregates::Price>(),

                            FactMetadata<PricePosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.PricePosition)
                                .HasDependentAggregate<Aggregates::Price>(Specs.Map.Facts.ToPriceAggregate.ByPricePosition),

                            // TODO: period
                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Project)
                    );

            Metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}