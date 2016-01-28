using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;

using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class FactsReplicationMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public FactsReplicationMetadataSource()
        {
            HierarchyMetadata factsReplicationMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Facts))
                    .Childs(FactMetadata<Activity>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Activities)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByActivity)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByClientActivity),

                            FactMetadata<Account>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Accounts)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByAccount),

                            FactMetadata<BranchOfficeOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByBranchOfficeOrganizationUnit),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Categories)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByCategory),

                            FactMetadata<CategoryFirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryFirmAddresses)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirmAddressCategory)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryFirmAddress)
                                .HasDependentAggregate<EntityTypeClient>(Specs.Map.Facts.ToClientAggregate.ByCategoryFirmAddress),

                            FactMetadata<CategoryGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryGroups)
                                .HasMatchedAggregate<EntityTypeCategoryGroup>()
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryGroup)
                                .HasDependentAggregate<EntityTypeClient>(Specs.Map.Facts.ToClientAggregate.ByCategoryGroup),

                            FactMetadata<CategoryOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryOrganizationUnits)
                                .HasDependentAggregate<EntityTypeProject>(Specs.Map.Facts.ToProjectAggregate.ByCategoryOrganizationUnit)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryOrganizationUnit)
                                .HasDependentAggregate<EntityTypeClient>(Specs.Map.Facts.ToClientAggregate.ByCategoryOrganizationUnit),

                            FactMetadata<Client>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Clients)
                                .HasMatchedAggregate<EntityTypeClient>()
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByClient),

                            FactMetadata<Contact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Contacts)
                                .HasDependentAggregate<EntityTypeClient>(Specs.Map.Facts.ToClientAggregate.ByContacts)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByContacts),

                            FactMetadata<Firm>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Firms)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirm)
                                .HasMatchedAggregate<EntityTypeFirm>()
                                .HasDependentAggregate<EntityTypeClient>(Specs.Map.Facts.ToClientAggregate.ByFirm),

                            FactMetadata<FirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmAddresses)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirmAddress)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByFirmAddress)
                                .HasDependentAggregate<EntityTypeClient>(Specs.Map.Facts.ToClientAggregate.ByFirmAddress),

                            FactMetadata<FirmContact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmContacts)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByFirmContacts),

                            FactMetadata<LegalPerson>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.LegalPersons)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByLegalPerson),

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Orders)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByOrder),

                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Projects)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByProject)
                                .HasMatchedAggregate<EntityTypeProject>()
                                .HasDependentAggregate<EntityTypeTerritory>(Specs.Map.Facts.ToTerritoryAggregate.ByProject)
                                .HasDependentAggregate<EntityTypeFirm>(Specs.Map.Facts.ToFirmAggregate.ByProject),

                            FactMetadata<Territory>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Territories)
                                .HasMatchedAggregate<EntityTypeTerritory>(),

                            FactMetadata<SalesModelCategoryRestriction>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.SalesModelCategoryRestrictions)
                                .HasDependentAggregate<EntityTypeProject>(Specs.Map.Facts.ToProjectAggregate.BySalesModelCategoryRestriction)

                                );

            _metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}