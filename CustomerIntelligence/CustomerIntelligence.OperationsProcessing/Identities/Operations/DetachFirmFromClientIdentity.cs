using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class DetachFirmFromClientIdentity : OperationIdentityBase<DetachFirmFromClientIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DetachFirmFromClientIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Отвязать фирму от клиента";
            }
        }
    }
}
