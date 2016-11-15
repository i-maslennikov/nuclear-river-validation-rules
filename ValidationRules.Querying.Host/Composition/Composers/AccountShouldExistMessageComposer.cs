using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AccountShouldExistMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AccountShouldExist;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "Заказ не имеет привязки к лицевому счёту");
        }
    }
}