using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DefaultThemeMustBeExactlyOneMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var projectReference = validationResult.ReadProjectReference();
            var themeCount = validationResult.ReadProjectThemeCount();

            var themeCountMessage = themeCount == 0 ? Resources.DefaultThemeIsNotSpecified : Resources.MoreThanOneDefaultTheme;

            return new MessageComposerResult(
                projectReference,
                themeCountMessage,
                projectReference);
        }
    }
}