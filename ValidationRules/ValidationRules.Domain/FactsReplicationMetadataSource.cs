using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.ValidationRules.Domain.EntityTypes;
using NuClear.ValidationRules.Domain.Model.Facts;
using NuClear.ValidationRules.Domain.Specifications;

namespace NuClear.ValidationRules.Domain
{
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
                                .HasDependentEntity(EntityTypePrice.Instance, Specs.Map.Facts.ToPriceAggregate.ByAssociatedPosition),

                            FactMetadata<AssociatedPositionsGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.AssociatedPositionsGroup)
                                .HasDependentEntity(EntityTypePrice.Instance, Specs.Map.Facts.ToPriceAggregate.ByAssociatedPositionGroup),

                            FactMetadata<DeniedPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.DeniedPosition)
                                .HasDependentEntity(EntityTypePrice.Instance, Specs.Map.Facts.ToPriceAggregate.ByDeniedPosition),

                            FactMetadata<RulesetRule>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.RulesetRule)
                                .HasDependentEntity(EntityTypeRuleset.Instance, Specs.Map.Facts.ToRulesetAggregate.ByRulesetRule),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Category)
                                .HasDependentEntity(EntityTypeOrder.Instance, Specs.Map.Facts.ToOrderAggregate.ByCategory),

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Order)
                                .HasMatchedEntity(EntityTypeOrder.Instance),

                            FactMetadata<OrderPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrderPosition)
                                .HasDependentEntity(EntityTypeOrder.Instance, Specs.Map.Facts.ToOrderAggregate.ByOrderPosition),

                            FactMetadata<OrderPositionAdvertisement>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrderPositionAdvertisement)
                                .HasDependentEntity(EntityTypeOrder.Instance, Specs.Map.Facts.ToOrderAggregate.ByOrderPositionAdvertisement),

                            FactMetadata<Position>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Position)
                                .HasMatchedEntity(EntityTypePosition.Instance)
                                .HasDependentEntity(EntityTypeOrder.Instance, Specs.Map.Facts.ToOrderAggregate.ByPosition),

                            FactMetadata<Price>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Price)
                                .HasMatchedEntity(EntityTypePrice.Instance),

                            FactMetadata<PricePosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.PricePosition)
                                .HasDependentEntity(EntityTypePrice.Instance, Specs.Map.Facts.ToPriceAggregate.ByPricePosition),

                            // TODO: attach to period aggregate
                            FactMetadata<OrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrganizationUnit),
                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Project)
                    );

            Metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}