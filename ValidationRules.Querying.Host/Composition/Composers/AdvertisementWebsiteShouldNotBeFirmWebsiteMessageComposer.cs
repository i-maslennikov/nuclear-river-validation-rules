using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsiteMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var website = validationResult.ReadWebsite();

            return new MessageComposerResult(
                orderReference,
                $"Для фирмы {{0}} заказана рекламная ссылка {website} позиция {{1}}, дублирующая контакт фирмы",
                firmReference,
                orderPositionReference);
        }
    }
}