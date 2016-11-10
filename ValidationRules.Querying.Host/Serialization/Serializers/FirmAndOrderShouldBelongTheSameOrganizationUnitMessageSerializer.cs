using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class FirmAndOrderShouldBelongTheSameOrganizationUnitMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(
                orderReference,
                "Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы");
        }
    }
}