using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DefaultThemeMustHaveOnlySelfAdsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustHaveOnlySelfAds;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageComposerResult(
                orderReference,
                "Установленная по умолчанию тематика {0} должна содержать только саморекламу",
                themeReference);
        }
    }
}