using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsiteMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementWebsiteShouldNotBeFirmWebsiteMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var website = message.ReadWebsite();

            return new MessageTemplate(
                orderReference,
                "Для фирмы {0} заказана рекламная ссылка {1} позиция {2}, дублирующая контакт фирмы",
                _linkFactory.CreateLink(firmReference),
                website,
                _linkFactory.CreateLink(orderPositionReference));
        }
    }
}