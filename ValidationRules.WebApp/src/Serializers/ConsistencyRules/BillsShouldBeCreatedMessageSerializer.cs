using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class BillsShouldBeCreatedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BillsShouldBeCreatedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BillsShouldBeCreated;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "Для заказа необходимо сформировать счета");
        }
    }
}