using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Dto;
using NuClear.ValidationRules.Domain.Model.Facts;
using NuClear.ValidationRules.Domain.Specifications;

namespace NuClear.ValidationRules.Domain
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class ImportOrderValidationConfigMetadataSource : MetadataSourceBase<ImportStatisticsMetadataIdentity>
    {
        public ImportOrderValidationConfigMetadataSource()
        {
            HierarchyMetadata importStatisticsMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ImportStatisticsMetadataIdentity>(ReplicationMetadataName.PriceContextConfig))
                    .Childs(ImportStatisticsMetadata<GlobalAssociatedPosition, OrderValidationConfig>
                                .Config
                                .HasSource(Specs.Map.Config.ToFacts.GlobalAssociatedPosition)
                                .Aggregated(dto => new FindSpecification<GlobalAssociatedPosition>(entity => true))
                                .FakeOperationsProvider(),

                            ImportStatisticsMetadata<GlobalDeniedPosition, OrderValidationConfig>
                                .Config
                                .HasSource(Specs.Map.Config.ToFacts.GlobalDeniedPosition)
                                .Aggregated(dto => new FindSpecification<GlobalDeniedPosition>(entity => true))
                                .FakeOperationsProvider());

            Metadata = new Dictionary<Uri, IMetadataElement> { { importStatisticsMetadataRoot.Identity.Id, importStatisticsMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}