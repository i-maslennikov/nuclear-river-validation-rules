using NuClear.Model.Common.Operations.Identity;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Operations
{
    public sealed class ManageGlobalAssociatedDeniedDraftRulesIdentity : OperationIdentityBase<ManageGlobalAssociatedDeniedDraftRulesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.ManageGlobalAssociatedDeniedDraftRulesIdentity;

        public override string Description => "Управление глобальными правилами сопутствия и запрещения";
    }
}