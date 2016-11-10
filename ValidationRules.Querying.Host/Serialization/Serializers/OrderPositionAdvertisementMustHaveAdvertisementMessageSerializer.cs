using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPositionAdvertisementMustHaveAdvertisementMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var positionReference = validationResult.ReadPositionReference();

            if (orderPositionReference.Name == positionReference.Name)
            {
                return new MessageSerializerResult(
                    orderReference,
                    "В позиции {0} необходимо указать рекламные материалы",
                    orderPositionReference);
            }

            return new MessageSerializerResult(
                orderReference,
                "В позиции {0} необходимо указать рекламные материалы для подпозиции {1}",
                orderPositionReference,
                positionReference);
        }
    }
}