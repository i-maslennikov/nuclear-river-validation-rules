using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPositionMustNotReferenceDeletedAdvertisementMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();

            return new MessageSerializerResult(
                orderReference,
                "В позиции {0} выбран удалённый рекламный материал {1}",
                orderPositionReference,
                advertisementReference);
        }
    }
}