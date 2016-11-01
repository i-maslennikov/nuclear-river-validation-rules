using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ThemeRules
{
    public sealed class DefaultThemeMustHaveOnlySelfAdsMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public DefaultThemeMustHaveOnlySelfAdsMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustHaveOnlySelfAds;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var themeReference = validationResult.ReadThemeReference();

            return new MessageTemplate(
                orderReference,
                "Установленная по умолчанию тематика {0} должна содержать только саморекламу",
                _linkFactory.CreateLink(themeReference));
        }
    }
}