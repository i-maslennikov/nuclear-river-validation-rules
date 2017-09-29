using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementCountPerThemeShouldBeLimitedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var projectReference = references.Get<EntityTypeProject>();
            var themeReference = references.Get<EntityTypeTheme>();
            var dto = extra.ReadOversalesMessage();

            return new MessageComposerResult(
                projectReference,
                Resources.AdvertisementCountPerThemeShouldBeLimited,
                themeReference,
                dto.Count,
                dto.Max);
        }
    }
}