using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class WhiteListAdvertisementMustPresentMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.WhiteListAdvertisementMustPresent;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementForWhitelistDoesNotSpecified,
                firmReference);
        }
    }
}