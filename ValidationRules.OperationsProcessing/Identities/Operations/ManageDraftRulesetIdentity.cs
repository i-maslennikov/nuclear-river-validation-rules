using NuClear.Model.Common.Operations.Identity;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Operations
{
    public sealed class ManageDraftRulesetIdentity : OperationIdentityBase<ManageDraftRulesetIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.ManageDraftRulesetIdentity;

        public override string Description => nameof(OperationIdentityIds.ManageDraftRulesetIdentity);
    }
}