using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionAdvertisementMustHaveAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderPosition = (OrderPositionNamedReference)references.GetMany<EntityTypeOrderPosition>().First();
            var orderPositionAdvertisement = references.GetMany<EntityTypeOrderPosition>().Last();

            if (orderPosition.Name == orderPositionAdvertisement.Name)
            {
                return new MessageComposerResult(
                    orderPosition.Order,
                    Resources.OrderCheckPositionMustHaveAdvertisements,
                    orderPosition);
            }

            return new MessageComposerResult(
                orderPosition.Order,
                string.Format(Resources.OrderCheckCompositePositionMustHaveAdvertisements, orderPositionAdvertisement.Name),
                orderPosition);
        }
    }
}