using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(
                orderReference,
                "Указана некорректная дата начала размещения");
        }
    }
}