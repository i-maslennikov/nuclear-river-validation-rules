using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class ChangeLeadFirmIdentity : OperationIdentityBase<ChangeLeadFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.ChangeLeadFirmIdentity;
        public override string Description => "Смена фирмы лида.";
    }
}