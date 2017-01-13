using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionAdvertisementMustBeCreatedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustBeCreated;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var positionReference = validationResult.ReadPositionReference();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckCompositePositionMustHaveLinkingObject,
                orderPositionReference,
                positionReference);
        }
    }
}