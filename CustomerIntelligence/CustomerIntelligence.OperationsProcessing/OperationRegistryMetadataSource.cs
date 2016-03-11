using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
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
                    .ExplicitEntityTypesMap(new Dictionary<IEntityType, IEntityType>
                    {
                        { EntityTypeAppointment.Instance,  EntityTypeActivity.Instance },
                        { EntityTypePhonecall.Instance,  EntityTypeActivity.Instance },
                        { EntityTypeTask.Instance, EntityTypeActivity.Instance },
                        { EntityTypeLetter.Instance, EntityTypeActivity.Instance }
                    })
                    .AllowedOperationIdentities(new HashSet<StrictOperationIdentity>
                    {
                        CreateIdentity.Instance.SpecificFor(EntityTypeOrder.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeClient.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeContact.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeOrderPosition.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeProject.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeTerritory.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeAccount.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeFirmAddress.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeFirmContact.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeLetter.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeTask.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypePhonecall.Instance),
                        CreateIdentity.Instance.SpecificFor(EntityTypeAppointment.Instance),

                        BulkCreateIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),
                        BulkCreateIdentity.Instance.SpecificFor(EntityTypeFirmAddress.Instance),
                        BulkCreateIdentity.Instance.SpecificFor(EntityTypeCategoryFirmAddress.Instance),
                        BulkCreateIdentity.Instance.SpecificFor(EntityTypeSalesModelCategoryRestriction.Instance),
                        BulkCreateIdentity.Instance.SpecificFor(EntityTypeLock.Instance),

                        UpdateIdentity.Instance.SpecificFor(EntityTypeOrder.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeAccount.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeTerritory.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeClient.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeContact.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeFirmAddress.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeFirmContact.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeBuilding.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeOrderPosition.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeProject.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeCategoryOrganizationUnit.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeBill.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeLetter.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeTask.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypePhonecall.Instance),
                        UpdateIdentity.Instance.SpecificFor(EntityTypeAppointment.Instance),

                        BulkUpdateIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),
                        BulkUpdateIdentity.Instance.SpecificFor(EntityTypeFirmContact.Instance),
                        BulkUpdateIdentity.Instance.SpecificFor(EntityTypeCategoryFirmAddress.Instance),

                        DeleteIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        DeleteIdentity.Instance.SpecificFor(EntityTypeAccount.Instance),
                        DeleteIdentity.Instance.SpecificFor(EntityTypeLegalPersonProfile.Instance),
                        DeleteIdentity.Instance.SpecificFor(EntityTypeOrderPosition.Instance),
                        DeleteIdentity.Instance.SpecificFor(EntityTypeCategoryFirmAddress.Instance),
                        DeleteIdentity.Instance.SpecificFor(EntityTypeContact.Instance),

                        BulkDeleteIdentity.Instance.SpecificFor(EntityTypeFirmContact.Instance),
                        BulkDeleteIdentity.Instance.SpecificFor(EntityTypeSalesModelCategoryRestriction.Instance),

                        ActivateIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),

                        DeactivateIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        DeactivateIdentity.Instance.SpecificFor(EntityTypeTerritory.Instance),
                        DeactivateIdentity.Instance.SpecificFor(EntityTypeClient.Instance),

                        BulkDeactivateIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),

                        AppendIdentity.Instance.SpecificFor(EntityTypeClient.Instance, EntityTypeClient.Instance),
                        DetachIdentity.Instance.SpecificFor(EntityTypeClient.Instance, EntityTypeFirm.Instance),

                        AssignIdentity.Instance.SpecificFor(EntityTypeClient.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeContact.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeAccount.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeDeal.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeOrder.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeLetter.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeTask.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypePhonecall.Instance),
                        AssignIdentity.Instance.SpecificFor(EntityTypeAppointment.Instance),

                        CancelIdentity.Instance.SpecificFor(EntityTypeLetter.Instance),
                        CancelIdentity.Instance.SpecificFor(EntityTypeTask.Instance),
                        CancelIdentity.Instance.SpecificFor(EntityTypePhonecall.Instance),
                        CancelIdentity.Instance.SpecificFor(EntityTypeAppointment.Instance),

                        CompleteIdentity.Instance.SpecificFor(EntityTypeLetter.Instance),
                        CompleteIdentity.Instance.SpecificFor(EntityTypeTask.Instance),
                        CompleteIdentity.Instance.SpecificFor(EntityTypePhonecall.Instance),
                        CompleteIdentity.Instance.SpecificFor(EntityTypeAppointment.Instance),

                        ReopenIdentity.Instance.SpecificFor(EntityTypeLetter.Instance),
                        ReopenIdentity.Instance.SpecificFor(EntityTypeTask.Instance),
                        ReopenIdentity.Instance.SpecificFor(EntityTypePhonecall.Instance),
                        ReopenIdentity.Instance.SpecificFor(EntityTypeAppointment.Instance),

                        MergeIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        MergeIdentity.Instance.SpecificFor(EntityTypeClient.Instance),

                        QualifyIdentity.Instance.SpecificFor(EntityTypeClient.Instance),
                        QualifyIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),

                        DisqualifyIdentity.Instance.SpecificFor(EntityTypeClient.Instance),
                        DisqualifyIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),

                        ChangeTerritoryIdentity.Instance.SpecificFor(EntityTypeClient.Instance),
                        ChangeTerritoryIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),

                        ChangeClientIdentity.Instance.SpecificFor(EntityTypeDeal.Instance),
                        ChangeClientIdentity.Instance.SpecificFor(EntityTypeFirm.Instance),
                        ChangeClientIdentity.Instance.SpecificFor(EntityTypeLegalPerson.Instance),
                        ChangeClientIdentity.Instance.SpecificFor(EntityTypeContact.Instance),

                        ActualizeOrderAmountToWithdrawIdentity.Instance.NonCoupled(),
                        SetInspectorIdentity.Instance.NonCoupled(),
                        ChangeDealIdentity.Instance.NonCoupled(),
                        CloseWithDenialIdentity.Instance.NonCoupled(),
                        ChangeOrderLegalPersonProfileIdentity.Instance.NonCoupled(),
                        CopyOrderIdentity.Instance.NonCoupled(),
                        RepairOutdatedIdentity.Instance.NonCoupled(),
                        ChangeRequisitesIdentity.Instance.NonCoupled(),
                        RevertWithdrawFromAccountsIdentity.Instance.NonCoupled(),
                        WithdrawFromAccountsIdentity.Instance.NonCoupled(),
                        CreateClientByFirmIdentity.Instance.NonCoupled(),
                        ApplyOrderDiscountIdentity.Instance.NonCoupled(),
                        ChangeOrderAccountIdentity.Instance.NonCoupled(),
                        ChangeOrderBargainIdentity.Instance.NonCoupled(),
                        ChangeOrderLegalPersonIdentity.Instance.NonCoupled(),
                        ClearOrderBargainIdentity.Instance.NonCoupled(),
                        UpdateOrderFinancialPerformanceIdentity.Instance.NonCoupled(),
                        CreateOrderBillsIdentity.Instance.NonCoupled(),
                        DeleteOrderBillsIdentity.Instance.NonCoupled(),
                        TransferLocksToAccountIdentity.Instance.NonCoupled(),
                        ImportCategoryOrganizationUnitIdentity.Instance.NonCoupled(),
                        SetMainFirmIdentity.Instance.NonCoupled(),
                        ActualizeActiveLocksIdentity.Instance.NonCoupled(),
                        ImportAdvModelInRubricInfoIdentity.Instance.NonCoupled(),

                        // эти операции станут disallowed после того как фирмы будем брать из InfoRussia
                        ImportCardForErmIdentity.Instance.NonCoupled(),
                        ImportCardIdentity.Instance.NonCoupled(),
                        ImportFirmIdentity.Instance.NonCoupled(),
                    })
                    .DisallowedOperationIdentities(new HashSet<StrictOperationIdentity>
                    {
                        ImportFirmPromisingIdentity.Instance.NonCoupled(),
                        CalculateClientPromisingIdentity.Instance.NonCoupled(),
                    })
                };


            Metadata = metadataElements.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}