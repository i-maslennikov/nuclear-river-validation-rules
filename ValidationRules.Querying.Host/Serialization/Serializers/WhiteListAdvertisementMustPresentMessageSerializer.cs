using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class WhiteListAdvertisementMustPresentMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMustPresent;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();

            return new MessageSerializerResult(
                orderReference,
                "Для фирмы {0} не указан рекламный материал в белый список",
                firmReference);
        }
    }
}