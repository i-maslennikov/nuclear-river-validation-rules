using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionCorrespontToInactivePositionMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionCorrespontToInactivePosition;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();

            return new MessageComposerResult(
                orderReference,
                "Позиция {0} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа.",
                orderPositionReference);
        }
    }
}