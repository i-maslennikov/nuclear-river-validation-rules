using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionMustNotReferenceDeletedAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();

            return new MessageComposerResult(
                orderReference,
                Resources.RemovedAdvertisemendSpecifiedForPosition,
                orderPositionReference,
                advertisementReference);
        }
    }
}