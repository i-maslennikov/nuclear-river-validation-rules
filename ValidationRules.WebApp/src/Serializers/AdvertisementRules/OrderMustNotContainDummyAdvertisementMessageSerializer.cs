using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class OrderMustNotContainDummyAdvertisementMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderMustNotContainDummyAdvertisementMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderMustNotContainDummyAdvertisement;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();

            return new MessageTemplate(
                orderReference,
                "Позиция {0} содержит заглушку рекламного материала",
                _linkFactory.CreateLink(orderPositionReference));
        }
    }
}