using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public sealed class OperationRegistryMetadataSource : MetadataSourceBase<OperationRegistryMetadataIdentity>
    {
        public OperationRegistryMetadataSource()
        {
            var metadataElements = new OperationRegistryMetadataElement[]
                {
                    OperationRegistryMetadataElement
                        .Config
                        .For<FactsSubDomain>()
                        .Allow<CreateIdentity, EntityTypeOrder>()
                        .Allow<CreateIdentity, EntityTypeClient>()
                        .Allow<CreateIdentity, EntityTypeContact>()
                        .Allow<CreateIdentity, EntityTypeLegalPerson>()
                        .Allow<CreateIdentity, EntityTypeOrderPosition>()
                        .Allow<CreateIdentity, EntityTypeProject>()
                        .Allow<CreateIdentity, EntityTypeTerritory>()
                        .Allow<CreateIdentity, EntityTypeAccount>()
                        .Allow<CreateIdentity, EntityTypeFirmAddress>()
                        .Allow<CreateIdentity, EntityTypeFirmContact>()
                        .Allow<CreateIdentity, EntityTypeLetter>()
                        .Allow<CreateIdentity, EntityTypeTask>()
                        .Allow<CreateIdentity, EntityTypePhonecall>()
                        .Allow<CreateIdentity, EntityTypeAppointment>()

                        .Allow<BulkCreateIdentity, EntityTypeFirm>()
                        .Allow<BulkCreateIdentity, EntityTypeFirmAddress>()
                        .Allow<BulkCreateIdentity, EntityTypeCategoryFirmAddress>()
                        .Allow<BulkCreateIdentity, EntityTypeSalesModelCategoryRestriction>()
                        .Allow<BulkCreateIdentity, EntityTypeLock>()

                        .Allow<UpdateIdentity, EntityTypeOrder>()
                        .Allow<UpdateIdentity, EntityTypeAccount>()
                        .Allow<UpdateIdentity, EntityTypeTerritory>()
                        .Allow<UpdateIdentity, EntityTypeLegalPerson>()
                        .Allow<UpdateIdentity, EntityTypeClient>()
                        .Allow<UpdateIdentity, EntityTypeContact>()
                        .Allow<UpdateIdentity, EntityTypeFirm>()
                        .Allow<UpdateIdentity, EntityTypeFirmAddress>()
                        .Allow<UpdateIdentity, EntityTypeFirmContact>()
                        .Allow<UpdateIdentity, EntityTypeBuilding>()
                        .Allow<UpdateIdentity, EntityTypeOrderPosition>()
                        .Allow<UpdateIdentity, EntityTypeProject>()
                        .Allow<UpdateIdentity, EntityTypeCategoryOrganizationUnit>()
                        .Allow<UpdateIdentity, EntityTypeBill>()
                        .Allow<UpdateIdentity, EntityTypeLetter>()
                        .Allow<UpdateIdentity, EntityTypeTask>()
                        .Allow<UpdateIdentity, EntityTypePhonecall>()
                        .Allow<UpdateIdentity, EntityTypeAppointment>()

                        .Allow<BulkUpdateIdentity, EntityTypeFirm>()
                        .Allow<BulkUpdateIdentity, EntityTypeFirmContact>()
                        .Allow<BulkUpdateIdentity, EntityTypeCategoryFirmAddress>()

                        .Allow<DeleteIdentity, EntityTypeLegalPerson>()
                        .Allow<DeleteIdentity, EntityTypeAccount>()
                        .Allow<DeleteIdentity, EntityTypeLegalPersonProfile>()
                        .Allow<DeleteIdentity, EntityTypeOrderPosition>()
                        .Allow<DeleteIdentity, EntityTypeCategoryFirmAddress>()
                        .Allow<DeleteIdentity, EntityTypeContact>()

                        .Allow<BulkDeleteIdentity, EntityTypeFirmContact>()
                        .Allow<BulkDeleteIdentity, EntityTypeSalesModelCategoryRestriction>()

                        .Allow<ActivateIdentity, EntityTypeLegalPerson>()

                        .Allow<DeactivateIdentity, EntityTypeLegalPerson>()
                        .Allow<DeactivateIdentity, EntityTypeTerritory>()
                        .Allow<DeactivateIdentity, EntityTypeClient>()

                        .Allow<BulkDeactivateIdentity, EntityTypeFirm>()

                        .Allow<AppendIdentity, EntityTypeClient, EntityTypeClient>()
                        .Allow<DetachIdentity, EntityTypeClient, EntityTypeFirm>()

                        .Allow<AssignIdentity, EntityTypeClient>()
                        .Allow<AssignIdentity, EntityTypeContact>()
                        .Allow<AssignIdentity, EntityTypeLegalPerson>()
                        .Allow<AssignIdentity, EntityTypeAccount>()
                        .Allow<AssignIdentity, EntityTypeFirm>()
                        .Allow<AssignIdentity, EntityTypeDeal>()
                        .Allow<AssignIdentity, EntityTypeOrder>()
                        .Allow<AssignIdentity, EntityTypeLetter>()
                        .Allow<AssignIdentity, EntityTypeTask>()
                        .Allow<AssignIdentity, EntityTypePhonecall>()
                        .Allow<AssignIdentity, EntityTypeAppointment>()

                        .Allow<CancelIdentity, EntityTypeLetter>()
                        .Allow<CancelIdentity, EntityTypeTask>()
                        .Allow<CancelIdentity, EntityTypePhonecall>()
                        .Allow<CancelIdentity, EntityTypeAppointment>()

                        .Allow<CompleteIdentity, EntityTypeLetter>()
                        .Allow<CompleteIdentity, EntityTypeTask>()
                        .Allow<CompleteIdentity, EntityTypePhonecall>()
                        .Allow<CompleteIdentity, EntityTypeAppointment>()

                        .Allow<ReopenIdentity, EntityTypeLetter>()
                        .Allow<ReopenIdentity, EntityTypeTask>()
                        .Allow<ReopenIdentity, EntityTypePhonecall>()
                        .Allow<ReopenIdentity, EntityTypeAppointment>()

                        .Allow<MergeIdentity, EntityTypeLegalPerson>()
                        .Allow<MergeIdentity, EntityTypeClient>()

                        .Allow<QualifyIdentity, EntityTypeClient>()
                        .Allow<QualifyIdentity, EntityTypeFirm>()

                        .Allow<DisqualifyIdentity, EntityTypeClient>()
                        .Allow<DisqualifyIdentity, EntityTypeFirm>()

                        .Allow<ChangeTerritoryIdentity, EntityTypeClient>()
                        .Allow<ChangeTerritoryIdentity, EntityTypeFirm>()

                        .Allow<ChangeClientIdentity, EntityTypeDeal>()
                        .Allow<ChangeClientIdentity, EntityTypeFirm>()
                        .Allow<ChangeClientIdentity, EntityTypeLegalPerson>()
                        .Allow<ChangeClientIdentity, EntityTypeContact>()

                        .Allow<ActualizeOrderAmountToWithdrawIdentity>()
                        .Allow<SetInspectorIdentity>()
                        .Allow<ChangeDealIdentity>()
                        .Allow<CloseWithDenialIdentity>()
                        .Allow<ChangeOrderLegalPersonProfileIdentity>()
                        .Allow<CopyOrderIdentity>()
                        .Allow<RepairOutdatedIdentity>()
                        .Allow<ChangeRequisitesIdentity>()
                        .Allow<RevertWithdrawFromAccountsIdentity>()
                        .Allow<WithdrawFromAccountsIdentity>()
                        .Allow<CreateClientByFirmIdentity>()
                        .Allow<ApplyOrderDiscountIdentity>()
                        .Allow<ChangeOrderAccountIdentity>()
                        .Allow<ChangeOrderBargainIdentity>()
                        .Allow<ChangeOrderLegalPersonIdentity>()
                        .Allow<ClearOrderBargainIdentity>()
                        .Allow<UpdateOrderFinancialPerformanceIdentity>()
                        .Allow<CreateOrderBillsIdentity>()
                        .Allow<DeleteOrderBillsIdentity>()
                        .Allow<TransferLocksToAccountIdentity>()
                        .Allow<ImportCategoryOrganizationUnitIdentity>()
                        .Allow<SetMainFirmIdentity>()
                        .Allow<ActualizeActiveLocksIdentity>()
                        .Allow<ImportAdvModelInRubricInfoIdentity>()

                        // эти операции станут ignored после того как фирмы будем брать из InfoRussia
                        .Allow<ImportCardForErmIdentity>()
                        .Allow<ImportCardIdentity>()
                        .Allow<ImportFirmIdentity>()

                        .Ignore<ImportFirmPromisingIdentity>()
                        .Ignore<CalculateClientPromisingIdentity>()
                };


            Metadata = metadataElements.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}