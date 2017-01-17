using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class BillsShouldBeCreatedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.BillsShouldBeCreated;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(orderReference, Resources.OrdersCheckNeedToCreateBills);
        }
    }
}