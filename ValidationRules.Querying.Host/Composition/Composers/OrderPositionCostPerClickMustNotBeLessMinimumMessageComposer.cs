using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionCostPerClickMustNotBeLessMinimumMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderPosition = (OrderPositionNamedReference)references.Get<EntityTypeOrderPosition>();
            var category = references.Get<EntityTypeCategory>();

            return new MessageComposerResult(
                orderPosition.Order,
                Resources.CpcIsTooSmall,
                orderPosition,
                category);
        }
    }
}