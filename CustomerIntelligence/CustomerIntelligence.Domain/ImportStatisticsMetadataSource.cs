using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    public class ImportDocumentMetadataSource : MetadataSourceBase<ImportDocumentMetadataIdentity>
    {
        public ImportDocumentMetadataSource()
        {
            HierarchyMetadata importStatisticsMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ImportDocumentMetadataIdentity>())
                    .Childs(ImportDocumentMetadata<FirmPopularity>
                                .Config
                                .ImportToFacts(Specs.Find.Bit.FirmCategoryStatistics.ByBitDto, Specs.Map.Bit.FirmCategoryStatistics())
                                .LeadsToProjectStatisticsCalculation(),

                            ImportDocumentMetadata<RubricPopularity>
                                .Config
                                .ImportToFacts(Specs.Find.Bit.ProjectCategoryStatistics.ByBitDto, Specs.Map.Bit.ProjectCategoryStatistics())
                                .LeadsToProjectStatisticsCalculation(),

                            ImportDocumentMetadata<FirmForecast>
                                .Config
                                .ImportToFacts(Specs.Find.Bit.FirmCategoryForecast.ByBitDto, Specs.Map.Bit.FirmCategoryForecasts())
                                .ImportToFacts(Specs.Find.Bit.FirmForecast.ByBitDto, Specs.Map.Bit.FirmForecasts())
                                .LeadsToProjectStatisticsCalculation());

            Metadata = new Dictionary<Uri, IMetadataElement> { { importStatisticsMetadataRoot.Identity.Id, importStatisticsMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}