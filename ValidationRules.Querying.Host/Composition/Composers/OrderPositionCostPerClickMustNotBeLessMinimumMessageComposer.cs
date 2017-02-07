using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionCostPerClickMustNotBeLessMinimumMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var orderPositionReference = references.Get("orderPosition");
            var categoryReference = references.Get("category");

            return new MessageComposerResult(
                orderReference,
                Resources.CpcIsTooSmall,
                orderPositionReference,
                categoryReference);
        }
    }
}