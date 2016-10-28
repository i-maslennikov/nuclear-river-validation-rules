using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class OrderPositionAdvertisementMustBeCreatedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPositionAdvertisementMustBeCreatedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustBeCreated;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var positionReference = message.ReadPositionReference();

            return new MessageTemplate(
                orderReference,
                "В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции {1}",
                _linkFactory.CreateLink(orderPositionReference),
                _linkFactory.CreateLink(positionReference));
        }
    }
}