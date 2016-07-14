using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class LocalizedMessageFactory
    {
        private readonly IDictionary<int, IMessageSerializer> _serializers
            = new IMessageSerializer[]
                {
                    new AccountBalanceShouldBePositiveMessageSerializer(),
                    new OrderPositionShouldCorrespontToActualPriceMessageSerializer(),
                    new OrderPositionsShouldCorrespontToActualPriceMessageSerializer(),
                    new DeniedPositionsCheckMessageSerializer(),
                    new AccountShouldExistMessageSerializer(),
                    new LockShouldNotExistMessageSerializer(),
                    new MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer(),
                    new AssociatedPositionsGroupCountMessageSerializer(),
                    new MinimumAdvertisementAmountMessageSerializer(),
                    new MaximumAdvertisementAmountMessageSerializer(),
                    new OrderPositionCorrespontToInactivePositionMessageSerializer(),
                }.ToDictionary(x => x.MessageType, x => x);

        public LocalizedMessage Localize(Version.ValidationResult result)
        {
            IMessageSerializer serializer;
            return _serializers.TryGetValue(result.MessageType, out serializer)
                       ? serializer.Serialize(result.MessageParams)
                       : new LocalizedMessage(new ResultBuilder(result.Result).WhenMassRelease(), $"Заказ {result.ReferenceId}", result.MessageParams.ToString());
        }
    }
}