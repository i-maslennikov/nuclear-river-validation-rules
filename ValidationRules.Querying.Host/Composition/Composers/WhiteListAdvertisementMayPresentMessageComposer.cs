using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class WhiteListAdvertisementMayPresentMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMayPresent;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementChoosenForWhitelist,
                firmReference,
                advertisementReference);
        }
    }
}