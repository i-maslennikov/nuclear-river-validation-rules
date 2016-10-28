using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class AdvertisementCountPerThemeShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementCountPerThemeShouldBeLimitedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageTemplate(orderReference,
                "Слишком много продаж в тематику {0}. Продано {1} позиций вместо {2} возможных",
                _linkFactory.CreateLink(themeReference),
                dto.Count,
                dto.Max);
        }
    }
}