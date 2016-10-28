using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AccountRules
{
    public sealed class AccountShouldExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AccountShouldExistMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AccountShouldExist;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "Заказ не имеет привязки к лицевому счёту");
        }
    }
}