using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.FirmRules
{
    public sealed class FirmAndOrderShouldBelongTheSameOrganizationUnitMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public FirmAndOrderShouldBelongTheSameOrganizationUnitMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageTemplate(orderReference,
                "Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы");
        }
    }
}