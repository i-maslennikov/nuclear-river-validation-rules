using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class QualifyClientIdentity : OperationIdentityBase<QualifyClientIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.QualifyClientIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Взятие клиента из резерва";
            }
        }
    }
}
