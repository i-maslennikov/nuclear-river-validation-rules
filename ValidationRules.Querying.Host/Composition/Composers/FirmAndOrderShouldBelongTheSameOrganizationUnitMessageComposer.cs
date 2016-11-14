using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmAndOrderShouldBelongTheSameOrganizationUnitMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы");
        }
    }
}