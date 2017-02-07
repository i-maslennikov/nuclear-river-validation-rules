using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DefaultThemeMustBeExactlyOneMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var projectReference = references.Get("project");
            var themeCount = message.ReadProjectThemeCount();

            var themeCountMessage = themeCount == 0 ? Resources.DefaultThemeIsNotSpecified : Resources.MoreThanOneDefaultTheme;

            return new MessageComposerResult(
                projectReference,
                themeCountMessage,
                projectReference);
        }
    }
}