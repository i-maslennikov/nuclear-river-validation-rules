using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class WhiteListAdvertisementMayPresentMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public WhiteListAdvertisementMayPresentMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMayPresent;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();
            var advertisementReference = message.ReadAdvertisementReference();

            return new MessageTemplate(orderReference,
                "Для фирмы {0} в белый список выбран рекламный материал {1}",
                _linkFactory.CreateLink(firmReference),
                _linkFactory.CreateLink(advertisementReference));
        }
    }
}