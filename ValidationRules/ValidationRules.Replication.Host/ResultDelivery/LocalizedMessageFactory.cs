using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers;

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
                    new LinkedObjectsMissedInPrincipalsMessageSerializer(),
                    new SatisfiedPrincipalPositionDifferentOrderMessageSerializer(),
                    new ConflictingPrincipalPositionMessageSerializer(),
                    new AssociatedPositionWithoutPrincipalMessageSerializer(),
                }.ToDictionary(x => x.MessageType, x => x);

        public LocalizedMessage Localize(Message result)
        {
            IMessageSerializer serializer;
            return _serializers.TryGetValue(result.MessageType, out serializer)
                       ? serializer.Serialize(result)
                       : new LocalizedMessage(result.GetLevel(), $"Неопознанная ошибка с кодом {result.MessageType}", result.Data.ToString());
        }
    }
}