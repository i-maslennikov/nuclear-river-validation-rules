using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class DisqualifyLeadIdentity : OperationIdentityBase<DisqualifyLeadIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.DisqualifyLeadIdentity;
        public override string Description => "Дисквалификация лида.";
    }
}