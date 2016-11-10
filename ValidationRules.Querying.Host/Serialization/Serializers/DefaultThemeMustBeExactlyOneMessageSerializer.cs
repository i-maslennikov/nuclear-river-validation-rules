using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class DefaultThemeMustBeExactlyOneMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var projectReference = validationResult.ReadProjectReference();
            var themeCount = validationResult.ReadProjectThemeCount();

            var themeCountMessage = themeCount == 0 ? "не указана тематика по умолчанию" : "установлено более одной тематики по умолчанию";

            return new MessageSerializerResult(
                projectReference,
                $"Для подразделения {{0}} {themeCountMessage}",
                projectReference);
        }
    }
}