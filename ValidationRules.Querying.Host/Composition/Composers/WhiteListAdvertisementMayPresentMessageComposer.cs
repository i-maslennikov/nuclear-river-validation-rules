using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class WhiteListAdvertisementMayPresentMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMayPresent;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var firmReference = references.Get<EntityTypeFirm>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();

            return new MessageComposerResult(
                firmReference,
                Resources.AdvertisementChoosenForWhitelist,
                firmReference,
                advertisementReference);
        }
    }
}