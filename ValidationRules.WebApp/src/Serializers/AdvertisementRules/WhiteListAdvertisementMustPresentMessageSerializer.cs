using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class WhiteListAdvertisementMustPresentMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public WhiteListAdvertisementMustPresentMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMustPresent;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();

            return new MessageTemplate(orderReference,
                "Для фирмы {0} не указан рекламный материал в белый список",
                _linkFactory.CreateLink(firmReference));
        }
    }
}