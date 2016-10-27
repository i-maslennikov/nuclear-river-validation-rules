using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class LegalPersonProfileWarrantyShouldNotBeExpiredMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LegalPersonProfileWarrantyShouldNotBeExpiredMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var legalPersonProfileReference = validationResult.ReadLegalPersonProfileReference();

            return new MessageTemplate(
                orderReference,
                "У юр. лица клиента, в профиле {0} указана доверенность с датой окончания действия раньше даты подписания заказа",
                _linkFactory.CreateLink(legalPersonProfileReference));
        }
    }
}