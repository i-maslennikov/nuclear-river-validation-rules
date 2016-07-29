using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class UnityLocalizedMessageFactory
    {
        private static readonly IReadOnlyCollection<Type> SerializerTypes = new[]
            {
                typeof(AccountBalanceShouldBePositiveMessageSerializer),
                typeof(OrderPositionShouldCorrespontToActualPriceMessageSerializer),
                typeof(OrderPositionsShouldCorrespontToActualPriceMessageSerializer),
                typeof(DeniedPositionsCheckMessageSerializer),
                typeof(AccountShouldExistMessageSerializer),
                typeof(LockShouldNotExistMessageSerializer),
                typeof(MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer),
                typeof(AssociatedPositionsGroupCountMessageSerializer),
                typeof(MinimumAdvertisementAmountMessageSerializer),
                typeof(MaximumAdvertisementAmountMessageSerializer),
                typeof(OrderPositionCorrespontToInactivePositionMessageSerializer),
                typeof(LinkedObjectsMissedInPrincipalsMessageSerializer),
                typeof(SatisfiedPrincipalPositionDifferentOrderMessageSerializer),
                typeof(ConflictingPrincipalPositionMessageSerializer),
                typeof(AssociatedPositionWithoutPrincipalMessageSerializer),
            };

        private readonly Dictionary<int, IMessageSerializer> _serializers;

        public UnityLocalizedMessageFactory(IUnityContainer container)
        {
            _serializers = SerializerTypes.Select(x => (IMessageSerializer)container.Resolve(x)).ToDictionary(x => x.MessageType, x => x);
        }

        public LocalizedMessage Localize(Message result)
        {
            IMessageSerializer serializer;
            return _serializers.TryGetValue(result.MessageType, out serializer)
                       ? serializer.Serialize(result)
                       : new LocalizedMessage(result.GetLevel(), $"Неопознанная ошибка с кодом {result.MessageType}", result.Data.ToString());
        }
    }
}