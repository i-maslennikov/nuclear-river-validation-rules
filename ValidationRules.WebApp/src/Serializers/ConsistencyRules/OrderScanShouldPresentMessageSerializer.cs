using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class OrderScanShouldPresentMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderScanShouldPresentMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderScanShouldPresent;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(
                orderReference,
                "Отсутствует сканированная копия Бланка заказа");
        }
    }
}