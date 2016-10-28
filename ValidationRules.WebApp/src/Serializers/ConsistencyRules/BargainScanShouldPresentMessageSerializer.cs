using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class BargainScanShouldPresentMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BargainScanShouldPresentMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BargainScanShouldPresent;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "Отсутствует сканированная копия договора");
        }
    }
}