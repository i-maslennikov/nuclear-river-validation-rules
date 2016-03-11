using NuClear.Model.Common.Operations.Identity;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Operations
{
    public sealed class PublishGlobalAssociatedDeniedRulesIdentity : OperationIdentityBase<PublishGlobalAssociatedDeniedRulesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.PublishGlobalAssociatedDeniedRulesIdentity;

        public override string Description => "Публикация глобальных правил сопутствия и запрещения";
    }
}
