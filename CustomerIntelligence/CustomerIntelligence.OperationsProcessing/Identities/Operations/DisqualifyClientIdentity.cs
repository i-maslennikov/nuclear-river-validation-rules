using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class DisqualifyClientIdentity : OperationIdentityBase<DisqualifyClientIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.DisqualifyClientIdentity;

        public override string Description => "Вернуть клиента в резерв";
    }
}