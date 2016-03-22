using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class ChangeOrderReleaseCountIdentity : OperationIdentityBase<ChangeOrderReleaseCountIdentity>, INonCoupledOperationIdentity
    {
        public override int Id => OperationIdentityIds.ChangeOrderReleaseCountIdentity;
        public override string Description => "Изменение количества периодов размещения заказа";
    }
}