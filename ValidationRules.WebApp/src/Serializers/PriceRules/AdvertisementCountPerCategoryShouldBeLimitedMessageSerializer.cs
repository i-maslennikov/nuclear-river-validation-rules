using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementCountPerCategoryShouldBeLimitedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var dto = validationResult.ReadOversalesMessage();

            return new MessageTemplate(orderReference,
                "В рубрику {0} заказано слишком много объявлений: Заказано {1}, допустимо не более {2}.",
                _linkFactory.CreateLink(categoryReference),
                dto.Count,
                dto.Max);
        }
    }
}