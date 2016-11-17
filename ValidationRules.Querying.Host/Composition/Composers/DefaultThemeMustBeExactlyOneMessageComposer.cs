using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DefaultThemeMustBeExactlyOneMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var projectReference = validationResult.ReadProjectReference();
            var themeCount = validationResult.ReadProjectThemeCount();

            var themeCountMessage = themeCount == 0 ? "не указана тематика по умолчанию" : "установлено более одной тематики по умолчанию";

            return new MessageComposerResult(
                projectReference,
                $"Для подразделения {{0}} {themeCountMessage}",
                projectReference);
        }
    }
}