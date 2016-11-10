using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPositionAdvertisementMustBeCreatedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustBeCreated;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var positionReference = validationResult.ReadPositionReference();

            return new MessageSerializerResult(
                orderReference,
                "В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции {1}",
                orderPositionReference,
                positionReference);
        }
    }
}