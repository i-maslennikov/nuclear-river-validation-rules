using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LockShouldNotExistMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LockShouldNotExist;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "Заказ имеет созданную блокировку на указанный период");
        }
    }
}