using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AdvertisementMustBelongToFirmMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementMustBelongToFirm;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var firmReference = validationResult.ReadFirmReference();

            return new MessageSerializerResult(
                orderReference,
                "В позиции {0} выбран рекламный материал {1}, не принадлежащий фирме {2}",
                orderPositionReference,
                advertisementReference,
                firmReference);
        }
    }
}