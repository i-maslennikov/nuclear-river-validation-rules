using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementMustBelongToFirmMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementMustBelongToFirm;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();
            var firmReference = validationResult.ReadFirmReference();

            return new MessageComposerResult(
                orderReference,
                "В позиции {0} выбран рекламный материал {1}, не принадлежащий фирме {2}",
                orderPositionReference,
                advertisementReference,
                firmReference);
        }
    }
}