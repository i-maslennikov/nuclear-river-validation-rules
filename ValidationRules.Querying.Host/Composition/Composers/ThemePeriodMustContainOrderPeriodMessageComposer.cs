using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ThemePeriodMustContainOrderPeriodMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemePeriodMustContainOrderPeriod;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageComposerResult(
                orderReference,
                Resources.ThemePeriodDoesNotOverlapOrderPeriod,
                orderReference,
                themeReference);
        }
    }
}