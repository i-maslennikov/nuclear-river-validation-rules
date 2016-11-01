using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ThemeRules
{
    public sealed class DefaultThemeMustBeExactlyOneMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public DefaultThemeMustBeExactlyOneMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var projectReference = validationResult.ReadProjectReference();
            var themeCount = validationResult.ReadProjectThemeCount();

            return new MessageTemplate(
                projectReference,
                "Для подразделения {0} {1}",
                _linkFactory.CreateLink(projectReference),
                themeCount == 0 ? "не указана тематика по умолчанию" : "установлено более одной тематики по умолчанию");
        }
    }
}