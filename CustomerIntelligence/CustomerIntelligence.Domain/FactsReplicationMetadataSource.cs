using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model.Operations;

using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;
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
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByActivity)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByClientActivity),

                            FactMetadata<Account>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Accounts)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByAccount),

                            FactMetadata<BranchOfficeOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByBranchOfficeOrganizationUnit),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Categories)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByCategory),

                            FactMetadata<CategoryFirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryFirmAddresses)
                                .HasDependentEntity<Statistics::ProjectStatistics, StatisticsKey>(Specs.Map.Facts.ToStatistics.ByFirmAddressCategory)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByCategoryFirmAddress)
                                .HasDependentEntity<CI::Client, long>(Specs.Map.Facts.ToClientAggregate.ByCategoryFirmAddress),

                            FactMetadata<CategoryGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryGroups)
                                .HasMatchedEntity<CI::CategoryGroup>()
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByCategoryGroup)
                                .HasDependentEntity<CI::Client, long>(Specs.Map.Facts.ToClientAggregate.ByCategoryGroup),

                            FactMetadata<CategoryOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryOrganizationUnits)
                                .HasDependentEntity<CI::Project, long>(Specs.Map.Facts.ToProjectAggregate.ByCategoryOrganizationUnit)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByCategoryOrganizationUnit)
                                .HasDependentEntity<CI::Client, long>(Specs.Map.Facts.ToClientAggregate.ByCategoryOrganizationUnit),

                            FactMetadata<Client>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Clients)
                                .HasMatchedEntity<CI::Client>()
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByClient),

                            FactMetadata<Contact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Contacts)
                                .HasDependentEntity<CI::Client, long>(Specs.Map.Facts.ToClientAggregate.ByContacts)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByContacts),

                            FactMetadata<Firm>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Firms)
                                .HasMatchedEntity<CI::Firm>()
                                .HasDependentEntity<Statistics::ProjectStatistics, StatisticsKey>(Specs.Map.Facts.ToStatistics.ByFirm)
                                .HasDependentEntity<CI::Client, long>(Specs.Map.Facts.ToClientAggregate.ByFirm),

                            FactMetadata<FirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmAddresses)
                                .HasDependentEntity<Statistics::ProjectStatistics, StatisticsKey>(Specs.Map.Facts.ToStatistics.ByFirmAddress)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByFirmAddress)
                                .HasDependentEntity<CI::Client, long>(Specs.Map.Facts.ToClientAggregate.ByFirmAddress),

                            FactMetadata<FirmContact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmContacts)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByFirmContacts),

                            FactMetadata<LegalPerson>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.LegalPersons)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByLegalPerson),

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Orders)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByOrder),

                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Projects)
                                .HasMatchedEntity<CI::Project>()
                                .HasDependentEntity<Statistics::ProjectStatistics, StatisticsKey>(Specs.Map.Facts.ToStatistics.ByProject)
                                .HasDependentEntity<CI::Territory, long>(Specs.Map.Facts.ToTerritoryAggregate.ByProject)
                                .HasDependentEntity<CI::Firm, long>(Specs.Map.Facts.ToFirmAggregate.ByProject),

                            FactMetadata<Territory>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Territories)
                                .HasMatchedEntity<CI::Territory>(),

                            FactMetadata<SalesModelCategoryRestriction>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.SalesModelCategoryRestrictions)
                                .HasDependentEntity<CI::Project, long>(Specs.Map.Facts.ToProjectAggregate.BySalesModelCategoryRestriction));

            _metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}