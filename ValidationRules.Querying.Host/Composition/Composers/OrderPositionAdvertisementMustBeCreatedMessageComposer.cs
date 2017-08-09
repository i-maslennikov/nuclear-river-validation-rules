using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionAdvertisementMustBeCreatedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustBeCreated;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderPosition = (OrderPositionNamedReference)references.GetMany<EntityTypeOrderPosition>().First();
            var orderPositionAdvertisement = references.GetMany<EntityTypeOrderPosition>().Last();

            return new MessageComposerResult(
                orderPosition.Order,
                string.Format(Resources.OrderCheckCompositePositionMustHaveLinkingObject, orderPositionAdvertisement.Name),
                orderPosition);
        }
    }
}