using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.FirmRules
{
    public sealed class FirmShouldHaveLimitedCategoryCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public FirmShouldHaveLimitedCategoryCountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.FirmShouldHaveLimitedCategoryCount;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var categoryCount = validationResult.ReadCategoryCount();

            return new MessageTemplate(
                orderReference,
                "Для фирмы {0} задано слишком большое число рубрик - {1}. Максимально допустимое - {2}",
                _linkFactory.CreateLink(firmReference),
                categoryCount.Actual,
                categoryCount.Allowed);
        }
    }
}