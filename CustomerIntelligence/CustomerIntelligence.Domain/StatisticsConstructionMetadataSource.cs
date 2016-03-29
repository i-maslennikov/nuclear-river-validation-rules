using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.CustomerIntelligence.Domain.Model.Statistics;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain
{
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