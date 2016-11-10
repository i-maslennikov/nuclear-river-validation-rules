using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderMustNotContainDummyAdvertisementMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustNotContainDummyAdvertisement;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageSerializerResult(
                orderReference,
                "Позиция {0} содержит заглушку рекламного материала",
                orderPositionReference);
        }
    }
}