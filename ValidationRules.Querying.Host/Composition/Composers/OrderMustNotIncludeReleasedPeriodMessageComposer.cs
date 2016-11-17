using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustNotIncludeReleasedPeriodMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustNotIncludeReleasedPeriod;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "Заказ оформлен на период, по которому уже сформирована сборка. Необходимо указать другие даты размещения заказа");
        }
    }
}