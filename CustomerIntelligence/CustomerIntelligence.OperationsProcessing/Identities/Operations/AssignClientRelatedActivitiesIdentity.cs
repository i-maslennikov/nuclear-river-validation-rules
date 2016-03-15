using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    public class AssignClientRelatedActivitiesIdentity : OperationIdentityBase<AssignClientRelatedActivitiesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.AssignClientRelatedActivitiesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена куратора действий клиента";
            }
        }
    }
}
