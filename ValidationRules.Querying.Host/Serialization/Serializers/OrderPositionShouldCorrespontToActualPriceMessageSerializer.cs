using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPositionShouldCorrespontToActualPriceMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionShouldCorrespontToActualPrice;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageSerializerResult(
                orderReference,
                "Позиция {0} не соответствует актуальному прайс-листу. Необходимо указать позицию из текущего действующего прайс-листа",
                orderPositionReference);
        }
    }
}
