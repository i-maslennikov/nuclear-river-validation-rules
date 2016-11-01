using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ThemeRules
{
    public sealed class ThemeCategoryMustBeActiveAndNotDeletedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public ThemeCategoryMustBeActiveAndNotDeletedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var themeReference = validationResult.ReadThemeReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageTemplate(
                themeReference,
                "Тематика {0} использует удаленную рубрику {1}",
                _linkFactory.CreateLink(themeReference),
                _linkFactory.CreateLink(categoryReference));
        }
    }
}