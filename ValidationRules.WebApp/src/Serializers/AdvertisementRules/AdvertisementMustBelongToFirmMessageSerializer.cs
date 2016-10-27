using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class AdvertisementMustBelongToFirmMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementMustBelongToFirmMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementMustBelongToFirm;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var firmReference = message.ReadFirmReference();

            return new MessageTemplate(orderReference,
                "В позиции {0} выбран рекламный материал {1}, не принадлежащий фирме {2}",
                _linkFactory.CreateLink(orderPositionReference),
                _linkFactory.CreateLink(advertisementReference),
                _linkFactory.CreateLink(firmReference));
        }
    }
}