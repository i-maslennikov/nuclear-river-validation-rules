using NuClear.ValidationRules.Querying.Host.Properties;
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
                Resources.AdvertisementSpecifiedForPositionDoesNotBelongToFirm,
                orderPositionReference,
                advertisementReference,
                firmReference);
        }
    }
}