using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AccountRules
{
    public sealed class AccountBalanceShouldBePositiveMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AccountBalanceShouldBePositiveMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AccountBalanceShouldBePositive;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAccountBalanceMessage();

            return new MessageTemplate(orderReference,
                "Для оформления заказа недостаточно средств. Необходимо: {0}. Имеется: {1}. Необходим лимит: {2}",
                dto.Planned,
                dto.Available,
                dto.Required);
        }
    }
}