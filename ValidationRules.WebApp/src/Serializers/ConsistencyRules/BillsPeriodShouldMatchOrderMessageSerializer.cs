using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class BillsPeriodShouldMatchOrderMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BillsPeriodShouldMatchOrderMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BillsPeriodShouldMatchOrder;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "Период размещения, указанный в заказе и в счете не совпадают");
        }
    }
}