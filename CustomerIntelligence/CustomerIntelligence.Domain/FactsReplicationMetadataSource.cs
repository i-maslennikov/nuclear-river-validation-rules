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

using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
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
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByActivity)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByClientActivity),

                            FactMetadata<Account>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Accounts)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByAccount),

                            FactMetadata<BranchOfficeOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByBranchOfficeOrganizationUnit),

                            FactMetadata<Category>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Categories)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategory),

                            FactMetadata<CategoryFirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryFirmAddresses)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirmAddressCategory)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryFirmAddress)
                                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByCategoryFirmAddress),

                            FactMetadata<CategoryGroup>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryGroups)
                                .HasMatchedAggregate<CI::CategoryGroup>()
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryGroup)
                                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByCategoryGroup),

                            FactMetadata<CategoryOrganizationUnit>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.CategoryOrganizationUnits)
                                .HasDependentAggregate<CI::Project>(Specs.Map.Facts.ToProjectAggregate.ByCategoryOrganizationUnit)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategoryOrganizationUnit)
                                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByCategoryOrganizationUnit),

                            FactMetadata<Client>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Clients)
                                .HasMatchedAggregate<CI::Client>()
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByClient),

                            FactMetadata<Contact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Contacts)
                                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByContacts)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByContacts),

                            FactMetadata<Firm>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Firms)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirm)
                                .HasMatchedAggregate<CI::Firm>()
                                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByFirm),

                            FactMetadata<FirmAddress>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmAddresses)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirmAddress)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByFirmAddress)
                                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByFirmAddress),

                            FactMetadata<FirmContact>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.FirmContacts)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByFirmContacts),

                            FactMetadata<LegalPerson>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.LegalPersons)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByLegalPerson),

                            FactMetadata<Order>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Orders)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByOrder),

                            FactMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Projects)
                                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByProject)
                                .HasMatchedAggregate<CI::Project>()
                                .HasDependentAggregate<CI::Territory>(Specs.Map.Facts.ToTerritoryAggregate.ByProject)
                                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByProject),

                            FactMetadata<Territory>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.Territories)
                                .HasMatchedAggregate<CI::Territory>(),

                            FactMetadata<SalesModelCategoryRestriction>
                                .Config
                                .HasSource(Specs.Map.Erm.ToFacts.SalesModelCategoryRestrictions)
                                .HasDependentAggregate<CI::Project>(Specs.Map.Facts.ToProjectAggregate.BySalesModelCategoryRestriction)

                                );

            _metadata = new Dictionary<Uri, IMetadataElement> { { factsReplicationMetadataRoot.Identity.Id, factsReplicationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}