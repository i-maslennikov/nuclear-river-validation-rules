using NuClear.Model.Common.Operations.Identity;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Operations
{
    public sealed class PublishRulesetIdentity : OperationIdentityBase<PublishRulesetIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.PublishRulesetIdentity;

        public override string Description => nameof(OperationIdentityIds.PublishRulesetIdentity);
    }
}
