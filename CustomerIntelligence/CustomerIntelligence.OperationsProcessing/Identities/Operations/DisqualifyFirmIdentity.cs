using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class DisqualifyFirmIdentity : OperationIdentityBase<DisqualifyFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.DisqualifyFirmIdentity;

        public override string Description => "Вернуть фирму в резерв";
    }
}
