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
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ImportDocumentMetadataSource()
        {
            HierarchyMetadata importStatisticsMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ImportDocumentMetadataIdentity>())
                    .Childs(ImportDocumentMetadata<FirmStatisticsDto>
                                .Config
                                .ImportToFacts(Specs.Find.Bit.FirmCategoryStatistics.ByBitDto, Specs.Map.Bit.FirmCategoryStatistics())
                                .LeadsToProjectStatisticsCalculation(),

                            ImportDocumentMetadata<CategoryStatisticsDto>
                                .Config
                                .ImportToFacts(Specs.Find.Bit.ProjectCategoryStatistics.ByBitDto, Specs.Map.Bit.ProjectCategoryStatistics())
                                .LeadsToProjectStatisticsCalculation(),

                            ImportDocumentMetadata<FirmForecastDto>
                                .Config
                                .ImportToFacts(Specs.Find.Bit.FirmCategoryForecast.ByBitDto, Specs.Map.Bit.FirmCategoryForecasts())
                                .ImportToFacts(Specs.Find.Bit.FirmForecast.ByBitDto, Specs.Map.Bit.FirmForecasts())
                                .LeadsToProjectStatisticsCalculation());

            _metadata = new Dictionary<Uri, IMetadataElement> { { importStatisticsMetadataRoot.Identity.Id, importStatisticsMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
            => _metadata;
    }
}