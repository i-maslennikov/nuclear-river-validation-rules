using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AccountRules
{
    public sealed class LockShouldNotExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LockShouldNotExistMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LockShouldNotExist;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(
                orderReference,
                "Заказ имеет созданную блокировку на указанный период");
        }
    }
}