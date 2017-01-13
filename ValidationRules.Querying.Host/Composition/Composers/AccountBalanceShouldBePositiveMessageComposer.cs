using NuClear.ValidationRules.Querying.Host.Properties;
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

            return new MessageComposerResult(orderReference, string.Format(Resources.OrdersCheckOrderInsufficientFunds, dto.Planned, dto.Available));
        }
    }
}