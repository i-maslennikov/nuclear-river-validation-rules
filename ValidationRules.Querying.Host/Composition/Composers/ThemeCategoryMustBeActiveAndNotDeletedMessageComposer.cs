using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ThemeCategoryMustBeActiveAndNotDeletedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var themeReference = validationResult.ReadThemeReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageComposerResult(
                themeReference,
                Resources.ThemeUsesInactiveCategory,
                themeReference,
                categoryReference);
        }
    }
}