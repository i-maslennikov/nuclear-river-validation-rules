using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ThemeCategoryMustBeActiveAndNotDeletedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var themeReference = references.Get<EntityTypeTheme>();
            var categoryReference = references.Get<EntityTypeCategory>();

            return new MessageComposerResult(
                themeReference,
                Resources.ThemeUsesInactiveCategory,
                themeReference,
                categoryReference);
        }
    }
}