using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class WhiteListAdvertisementMayPresentMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMayPresent;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();

            return new MessageSerializerResult(
                orderReference,
                "Для фирмы {0} в белый список выбран рекламный материал {1}",
                firmReference,
                advertisementReference);
        }
    }
}