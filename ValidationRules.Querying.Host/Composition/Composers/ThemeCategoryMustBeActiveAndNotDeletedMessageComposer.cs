using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ThemeCategoryMustBeActiveAndNotDeletedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var themeReference = validationResult.ReadThemeReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageComposerResult(
                themeReference,
                "Тематика {0} использует удаленную рубрику {1}",
                themeReference,
                categoryReference);
        }
    }
}