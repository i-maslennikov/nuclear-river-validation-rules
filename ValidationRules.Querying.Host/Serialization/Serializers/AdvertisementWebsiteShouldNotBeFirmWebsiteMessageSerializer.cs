using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsiteMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var website = validationResult.ReadWebsite();

            return new MessageSerializerResult(
                orderReference,
                $"Для фирмы {{0}} заказана рекламная ссылка {website} позиция {{1}}, дублирующая контакт фирмы",
                firmReference,
                orderPositionReference);
        }
    }
}