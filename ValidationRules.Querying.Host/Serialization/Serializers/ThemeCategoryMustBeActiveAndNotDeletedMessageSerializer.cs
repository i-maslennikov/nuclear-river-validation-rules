using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class ThemeCategoryMustBeActiveAndNotDeletedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var themeReference = validationResult.ReadThemeReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageSerializerResult(
                themeReference,
                "Тематика {0} использует удаленную рубрику {1}",
                themeReference,
                categoryReference);
        }
    }
}