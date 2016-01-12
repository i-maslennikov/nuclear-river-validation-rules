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
                                .HasSource(Specs.Map.Erm.ToFacts.AssociatedPosition),

                            FactMetadata<AssociatedPositionsGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.AssociatedPositionsGroup),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Category),

                            FactMetadata<DeniedPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.DeniedPosition),

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Order),

                            FactMetadata<OrderPosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrderPosition),

                            FactMetadata<OrderPositionAdvertisement>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrderPositionAdvertisement),

                            FactMetadata<OrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.OrganizationUnit),

                            FactMetadata<Position>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Position),

                            FactMetadata<Price>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Price),

                            FactMetadata<PricePosition>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.PricePosition),

                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Project)
                    );

            Metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}