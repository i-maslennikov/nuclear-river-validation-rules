using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    [DataContract]
    public sealed class ActualizeOrderPlatformIdentity : OperationIdentityBase<ActualizeOrderPlatformIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeOrderPlatformIdentity; }
        }

        public override string Description
        {
            get { return "Акуализировать платформозависимые атрибуты заказа"; }
        }
    }
}