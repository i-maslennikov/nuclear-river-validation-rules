using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class OrderEndDistrubutionShouldBeLastSecondOfMonthMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderEndDistrubutionShouldBeLastSecondOfMonthMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderEndDistrubutionShouldBeLastSecondOfMonth;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(
                orderReference,
                "Указана некорректная дата окончания размещения");
        }
    }
}