using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DefaultThemeMustHaveOnlySelfAdsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustHaveOnlySelfAds;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageComposerResult(
                orderReference,
                Resources.DeafaultThemeMustContainOnlySelfAds,
                themeReference);
        }
    }
}