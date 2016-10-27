using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class OrderPositionCorrespontToInactivePositionMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderPositionCorrespontToInactivePositionMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionCorrespontToInactivePosition;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageTemplate(
                orderReference,
                "Позиция {0} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа.",
                _linkFactory.CreateLink(orderPositionReference));
        }
    }
}