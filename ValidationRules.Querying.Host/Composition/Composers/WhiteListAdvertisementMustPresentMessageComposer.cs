using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class WhiteListAdvertisementMustPresentMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMustPresent;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementForWhitelistDoesNotSpecified,
                firmReference);
        }
    }
}