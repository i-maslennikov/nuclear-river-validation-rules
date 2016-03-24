using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Model.Statistics;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model.Operations;

using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class AggregateConstructionMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        public AggregateConstructionMetadataSource()
        {
            HierarchyMetadata aggregateConstructionMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Aggregates))
                    .Childs(AggregateMetadata<Firm, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Firms)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories1, Specs.Find.CI.FirmCategories1)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories2, Specs.Find.CI.FirmCategories2)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmTerritories, Specs.Find.CI.FirmTerritories),

                            AggregateMetadata<Client, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Clients)
                                .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts),

                            AggregateMetadata<Project, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Projects)
                                .HasValueObject(Specs.Map.Facts.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories),

                            AggregateMetadata<Territory, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Territories),

                            AggregateMetadata<CategoryGroup, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.CategoryGroups));

            Metadata = new Dictionary<Uri, IMetadataElement> { { aggregateConstructionMetadataRoot.Identity.Id, aggregateConstructionMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class StatisticsConstructionMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        public StatisticsConstructionMetadataSource()
        {
            HierarchyMetadata aggregateConstructionMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Statistics))
                    .Childs(AggregateMetadata<ProjectStatistics, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToStatistics.ProjectStatistics)
                                .HasEntity<ProjectCategoryStatistics>(Specs.Find.CI.ProjectCategoryStatistics)
                                .HasValueObject(Specs.Map.Facts.ToStatistics.FirmForecast, Specs.Find.CI.FirmForecast),

                            AggregateMetadata<ProjectCategoryStatistics, StatisticsKey>
                                .Config
                                .HasSource(Specs.Map.Facts.ToStatistics.ProjectCategoryStatistics)
                                .HasValueObject(Specs.Map.Facts.ToStatistics.FirmCategory3, Specs.Find.CI.FirmCategory3));

            Metadata = new Dictionary<Uri, IMetadataElement> { { aggregateConstructionMetadataRoot.Identity.Id, aggregateConstructionMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}