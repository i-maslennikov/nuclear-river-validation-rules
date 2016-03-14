using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

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
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByActivity)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByClientActivity),

                            FactMetadata<Account>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Accounts)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByAccount),

                            FactMetadata<BranchOfficeOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByBranchOfficeOrganizationUnit),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Categories)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByCategory),

                            FactMetadata<CategoryFirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryFirmAddresses)
                                .HasDependentEntity(EntityTypeFirmCategoryStatistics.Instance, Specs.Map.Facts.ToStatistics.ByFirmAddressCategory)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByCategoryFirmAddress)
                                .HasDependentEntity(EntityTypeClient.Instance, Specs.Map.Facts.ToClientAggregate.ByCategoryFirmAddress),

                            FactMetadata<CategoryGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryGroups)
                                .HasMatchedEntity(EntityTypeCategoryGroup.Instance)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByCategoryGroup)
                                .HasDependentEntity(EntityTypeClient.Instance, Specs.Map.Facts.ToClientAggregate.ByCategoryGroup),

                            FactMetadata<CategoryOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryOrganizationUnits)
                                .HasDependentEntity(EntityTypeProject.Instance, Specs.Map.Facts.ToProjectAggregate.ByCategoryOrganizationUnit)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByCategoryOrganizationUnit)
                                .HasDependentEntity(EntityTypeClient.Instance, Specs.Map.Facts.ToClientAggregate.ByCategoryOrganizationUnit),

                            FactMetadata<Client>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Clients)
                                .HasMatchedEntity(EntityTypeClient.Instance)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByClient),

                            FactMetadata<Contact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Contacts)
                                .HasDependentEntity(EntityTypeClient.Instance, Specs.Map.Facts.ToClientAggregate.ByContacts)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByContacts),

                            FactMetadata<Firm>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Firms)
                                .HasMatchedEntity(EntityTypeFirm.Instance)
                                .HasDependentEntity(EntityTypeFirmCategoryStatistics.Instance, Specs.Map.Facts.ToStatistics.ByFirm)
                                .HasDependentEntity(EntityTypeClient.Instance, Specs.Map.Facts.ToClientAggregate.ByFirm),

                            FactMetadata<FirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmAddresses)
                                .HasDependentEntity(EntityTypeFirmCategoryStatistics.Instance, Specs.Map.Facts.ToStatistics.ByFirmAddress)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByFirmAddress)
                                .HasDependentEntity(EntityTypeClient.Instance, Specs.Map.Facts.ToClientAggregate.ByFirmAddress),

                            FactMetadata<FirmContact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmContacts)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByFirmContacts),

                            FactMetadata<LegalPerson>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.LegalPersons)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByLegalPerson),

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Orders)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByOrder),

                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Projects)
                                .HasMatchedEntity(EntityTypeProject.Instance)
                                .HasDependentEntity(EntityTypeFirmCategoryStatistics.Instance, Specs.Map.Facts.ToStatistics.ByProject)
                                .HasDependentEntity(EntityTypeTerritory.Instance, Specs.Map.Facts.ToTerritoryAggregate.ByProject)
                                .HasDependentEntity(EntityTypeFirm.Instance, Specs.Map.Facts.ToFirmAggregate.ByProject),

                            FactMetadata<Territory>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Territories)
                                .HasMatchedEntity(EntityTypeTerritory.Instance),

                            FactMetadata<SalesModelCategoryRestriction>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.SalesModelCategoryRestrictions)
                                .HasDependentEntity(EntityTypeProject.Instance, Specs.Map.Facts.ToProjectAggregate.BySalesModelCategoryRestriction));

            _metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}