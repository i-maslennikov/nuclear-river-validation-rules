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
                "В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции {1}",
                orderPositionReference,
                positionReference);
        }
    }
}