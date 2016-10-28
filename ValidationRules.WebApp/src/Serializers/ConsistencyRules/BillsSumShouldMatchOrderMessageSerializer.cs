using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class BillsSumShouldMatchOrderMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BillsSumShouldMatchOrderMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BillsSumShouldMatchOrder;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "Сумма по счетам не совпадает с планируемой суммой заказа");
        }
    }
}