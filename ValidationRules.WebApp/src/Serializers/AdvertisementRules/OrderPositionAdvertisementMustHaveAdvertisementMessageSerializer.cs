using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class OrderPositionAdvertisementMustHaveAdvertisementMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPositionAdvertisementMustHaveAdvertisementMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var positionReference = message.ReadPositionReference();

            if (orderPositionReference.Item2 == positionReference.Item2)
            {
                return new MessageTemplate(
                    orderReference,
                    "В позиции {0} необходимо указать рекламные материалы",
                    _linkFactory.CreateLink(orderPositionReference));
            }

            return new MessageTemplate(
                orderReference,
                "В позиции {0} необходимо указать рекламные материалы для подпозиции {1}",
                _linkFactory.CreateLink(orderPositionReference),
                _linkFactory.CreateLink(positionReference));
        }
    }
}