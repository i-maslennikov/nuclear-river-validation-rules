using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class ConflictingPrincipalPositionMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ConflictingPrincipalPosition;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var dependent = (OrderPositionNamedReference)references[0];
            var principal = (OrderPositionNamedReference)references[1];

            if (dependent.Order.Id != principal.Order.Id)
            {
                return new MessageComposerResult(
                    dependent.Order,
                    string.Format(Resources.ConflictingPrincipalPositionTemplate + Resources.OrderDescriptionTemplate, dependent.PositionPrefix, principal.PositionPrefix),
                    dependent,
                    principal,
                    principal.Order);
            }
            else
            {
                return new MessageComposerResult(
                    dependent.Order,
                    string.Format(Resources.ConflictingPrincipalPositionTemplate, dependent.PositionPrefix, principal.PositionPrefix),
                    dependent,
                    principal);
            }
        }
    }
}