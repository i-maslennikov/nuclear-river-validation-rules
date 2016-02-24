using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model;

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
                                .HasIdentityProvider(DefaultIdentityProvider.Instance)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories1, Specs.Find.CI.FirmCategories1)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories2, Specs.Find.CI.FirmCategories2)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmTerritories, Specs.Find.CI.FirmTerritories),

                            AggregateMetadata<Client, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Clients)
                                .HasIdentityProvider(DefaultIdentityProvider.Instance)
                                .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts),

                            AggregateMetadata<Project, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Projects)
                                .HasIdentityProvider(DefaultIdentityProvider.Instance)
                                .HasValueObject(Specs.Map.Facts.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories),

                            AggregateMetadata<Territory, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Territories)
                                .HasIdentityProvider(DefaultIdentityProvider.Instance),

                            AggregateMetadata<CategoryGroup, long>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.CategoryGroups)
                                .HasIdentityProvider(DefaultIdentityProvider.Instance));

            Metadata = new Dictionary<Uri, IMetadataElement> { { aggregateConstructionMetadataRoot.Identity.Id, aggregateConstructionMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}