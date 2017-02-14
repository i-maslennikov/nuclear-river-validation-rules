using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DefaultThemeMustHaveOnlySelfAdsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustHaveOnlySelfAds;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var themeReference = references.Get<EntityTypeTheme>();

            return new MessageComposerResult(
                orderReference,
                Resources.DeafaultThemeMustContainOnlySelfAds,
                themeReference);
        }
    }
}