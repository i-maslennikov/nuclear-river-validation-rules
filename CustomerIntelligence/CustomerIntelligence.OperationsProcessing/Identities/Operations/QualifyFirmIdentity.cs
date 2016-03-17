using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class QualifyFirmIdentity : OperationIdentityBase<QualifyFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.QualifyFirmIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Взятие фирмы из резерва";
            }
        }
    }
}
