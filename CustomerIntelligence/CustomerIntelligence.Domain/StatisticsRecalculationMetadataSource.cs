using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.Storage.API.Specifications;

using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;
using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class StatisticsRecalculationMetadataSource : MetadataSourceBase<StatisticsRecalculationMetadataIdentity>
    {
        public StatisticsRecalculationMetadataSource()
        {
            HierarchyMetadata statisticsRecalculationMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<StatisticsRecalculationMetadataIdentity>())
                    .Childs(StatisticsRecalculationMetadata<Statistics::FirmCategory3>
                                .Config
                                .HasSource(Specs.Map.Facts.ToStatistics.FirmCategory3)
                                .HasTarget(Specs.Map.CI.ToStatistics.FirmCategory3)
                                .HasFilter(
                                           (projectId, categoryIds) =>
                                           categoryIds == null
                                               ? Specs.Find.CI.FirmCategory3.ByProject(projectId)
                                               : Specs.Find.CI.FirmCategory3.ByProjectAndCategories(projectId, categoryIds)),

                            StatisticsRecalculationMetadata<Statistics::FirmForecast>
                                .Config
                                .HasSource(Specs.Map.Facts.ToStatistics.FirmForecast)
                                .HasTarget(Specs.Map.CI.ToStatistics.FirmForecast)
                                .HasFilter((projectId, categoryIds) =>
                                           categoryIds == null
                                               ? Specs.Find.CI.FirmForecast.ByProject(projectId)
                                               : new FindSpecification<Statistics::FirmForecast>(x => false)));

            Metadata = new Dictionary<Uri, IMetadataElement> { { statisticsRecalculationMetadataRoot.Identity.Id, statisticsRecalculationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}