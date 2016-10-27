using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class LegalPersonShouldHaveAtLeastOneProfileMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LegalPersonShouldHaveAtLeastOneProfileMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "У юр. лица клиента отсутствует профиль");
        }
    }
}