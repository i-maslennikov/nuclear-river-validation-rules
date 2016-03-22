using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Operations
{
    [DataContract]
    public sealed class ActualizeOrderAggregatedIndicatorsPersistedCacheIdentity : OperationIdentityBase<ActualizeOrderAggregatedIndicatorsPersistedCacheIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ActualizeOrderAggregatedIndicatorsPersistedCacheIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Актуализируем persistent cache по агрегированным показателям заказа";
            }
        }
    }
}