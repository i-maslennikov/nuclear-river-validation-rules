using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionAdvertisementMustBeCreatedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionAdvertisementMustBeCreated;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var orderPositionReference = references.GetMany("orderPosition").First();
            var positionReference = references.GetMany("orderPosition").Last();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckCompositePositionMustHaveLinkingObject,
                orderPositionReference,
                positionReference);
        }
    }
}