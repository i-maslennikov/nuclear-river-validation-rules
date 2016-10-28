using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class OrderShouldNotBeSignedBeforeBargainMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderShouldNotBeSignedBeforeBargainMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderShouldNotBeSignedBeforeBargain;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(
                orderReference,
                "Договор не может иметь дату подписания позднее даты подписания заказа");
        }
    }
}