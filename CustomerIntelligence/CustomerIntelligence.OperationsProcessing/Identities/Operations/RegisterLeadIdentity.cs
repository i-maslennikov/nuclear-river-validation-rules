using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class RegisterLeadIdentity : OperationIdentityBase<RegisterLeadIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.RegisterLeadIdentity;
        public override string Description => "Регистрация лида.";
    }
}