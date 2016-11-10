using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPositionCorrespontToInactivePositionMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionCorrespontToInactivePosition;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageSerializerResult(
                orderReference,
                "Позиция {0} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа.",
                orderPositionReference);
        }
    }
}