using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class MaximumAdvertisementAmountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public MaximumAdvertisementAmountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.MaximumAdvertisementAmount;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAdvertisementCountMessage();

            return new MessageTemplate(
                orderReference,
                "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {3:d} - {4}",
                dto.Name,
                dto.Min,
                dto.Max,
                dto.Month,
                dto.Count);
        }
    }
}