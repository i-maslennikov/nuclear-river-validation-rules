using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class DequeueLeadIdentity : OperationIdentityBase<DequeueLeadIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.DequeueLeadIdentity;
        public override string Description => "Взятие лида из очереди.";
    }
}