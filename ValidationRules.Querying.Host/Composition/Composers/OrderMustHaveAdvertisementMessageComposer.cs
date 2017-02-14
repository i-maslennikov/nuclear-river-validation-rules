using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveAdvertisement;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var advertisementReference = references.Get<EntityTypeAdvertisement>();
            var advertisementElementReference = references.Get<EntityTypeAdvertisementElement>();

            return new MessageComposerResult(
                orderReference,
                Resources.OrdersCheckPositionMustHaveAdvertisementElements,
                advertisementReference,
                advertisementElementReference);
        }
    }
}