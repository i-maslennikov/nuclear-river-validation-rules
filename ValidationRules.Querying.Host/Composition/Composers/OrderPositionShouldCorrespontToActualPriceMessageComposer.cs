using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionShouldCorrespontToActualPriceMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionShouldCorrespontToActualPrice;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckOrderPositionDoesntCorrespontToActualPrice,
                orderPositionReference);
        }
    }
}
