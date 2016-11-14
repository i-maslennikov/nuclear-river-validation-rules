using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ThemePeriodMustContainOrderPeriodMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemePeriodMustContainOrderPeriod;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageComposerResult(
                orderReference,
                "Заказ {0} не может иметь продаж в тематику {1}, поскольку тематика действует не весь период размещения заказа",
                orderReference,
                themeReference);
        }
    }
}