using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class OrderPeriodMustContainAdvertisementPeriodMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPeriodMustContainAdvertisementPeriodMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var advertisementReference = message.ReadAdvertisementReference();

            return new MessageTemplate(orderReference,
                "Период размещения рекламного материала {0}, выбранного в позиции {1} должен захватывать 5 дней от текущего месяца размещения",
                _linkFactory.CreateLink(advertisementReference),
                _linkFactory.CreateLink(orderPositionReference));
        }
    }
}