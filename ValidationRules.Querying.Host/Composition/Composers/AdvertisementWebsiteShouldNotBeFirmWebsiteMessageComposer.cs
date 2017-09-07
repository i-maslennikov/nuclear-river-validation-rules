using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsiteMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var website = extra.ReadWebsite();

            return new MessageComposerResult(
                orderReference,
                Resources.FirmContactContainsSponsoredLinkError,
                firmReference,
                orderPositionReference,
                orderReference,
                website);
        }
    }
}