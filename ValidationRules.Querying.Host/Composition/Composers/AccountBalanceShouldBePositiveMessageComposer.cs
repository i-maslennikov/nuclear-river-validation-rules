using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AccountBalanceShouldBePositiveMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AccountBalanceShouldBePositive;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAccountBalanceMessage();

            return new MessageComposerResult(orderReference,
                $"Для оформления заказа недостаточно средств. Необходимо: {dto.Planned}. Имеется: {dto.Available}. Необходим лимит: {dto.Required}");
        }
    }
}