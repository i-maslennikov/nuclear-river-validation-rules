using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class QualifyLeadIdentity : OperationIdentityBase<QualifyLeadIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.QualifyLeadIdentity;
        public override string Description => "Квалификация лида.";
    }
}