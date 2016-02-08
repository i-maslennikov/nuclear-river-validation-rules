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
        public AggregateConstructionMetadataSource()
        {
            var metadata = (HierarchyMetadata)HierarchyMetadata.Config
                                        .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.PriceContextAggregates))
                                        .Childs(
                AggregateMetadata<Price>
                .Config
                .HasSource(Specs.Map.Facts.ToAggregates.Prices)
                .HasValueObject(Specs.Map.Facts.ToAggregates.DeniedPositions, Specs.Find.Aggs.DeniedPositions)

                );

            Metadata = new Dictionary<Uri, IMetadataElement> { { metadata.Identity.Id, metadata} };
        }

        //public AggregateConstructionMetadataSource()
        //{
        //    HierarchyMetadata aggregateConstructionMetadataRoot =
        //        HierarchyMetadata
        //            .Config
        //            .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Aggregates))
        //            .Childs(AggregateMetadata<Firm>
        //                        .Config
        //                        .HasSource(Specs.Map.Facts.ToCI.Firms)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories1, Specs.Find.CI.FirmCategories1)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories2, Specs.Find.CI.FirmCategories2)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.FirmTerritories, Specs.Find.CI.FirmTerritories),

        //                    AggregateMetadata<Client>
        //                        .Config
        //                        .HasSource(Specs.Map.Facts.ToCI.Clients)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts),

        //                    AggregateMetadata<Project>
        //                        .Config
        //                        .HasSource(Specs.Map.Facts.ToCI.Projects)
        //                        .HasValueObject(Specs.Map.Facts.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories),

        //                    AggregateMetadata<Territory>
        //                        .Config
        //                        .HasSource(Specs.Map.Facts.ToCI.Territories),

        //                    AggregateMetadata<CategoryGroup>
        //                        .Config
        //                        .HasSource(Specs.Map.Facts.ToCI.CategoryGroups));

        //    Metadata = new Dictionary<Uri, IMetadataElement> { { aggregateConstructionMetadataRoot.Identity.Id, aggregateConstructionMetadataRoot } };
        //}

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}