using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AdvertisementMustBelongToFirmMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementMustBelongToFirm;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            var firmReference = references.Get<EntityTypeFirm>();

            return new MessageComposerResult(
                orderReference,
                Resources.AdvertisementMustBelongToFirm,
                orderPositionReference,
                advertisementReference,
                firmReference);
        }
    }
}
