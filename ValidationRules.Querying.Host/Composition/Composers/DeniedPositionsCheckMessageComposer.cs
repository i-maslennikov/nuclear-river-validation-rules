using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DeniedPositionsCheckMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmPositionMustNotHaveDeniedPositions;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var dependent = (OrderPositionNamedReference)references[0];
            var principal = (OrderPositionNamedReference)references[1];

            if (dependent.Order.Reference.Id != principal.Order.Reference.Id)
            {
                return new MessageComposerResult(
                    dependent.Order,
                    string.Format(Resources.ADPCheckModeSpecificOrder_MessageTemplate + Resources.OrderDescriptionTemplate, dependent.PositionPrefix, principal.PositionPrefix),
                    dependent,
                    principal,
                    principal.Order);
            }
            else
            {
                return new MessageComposerResult(
                    dependent.Order,
                    string.Format(Resources.ADPCheckModeSpecificOrder_MessageTemplate, dependent.PositionPrefix, principal.PositionPrefix),
                    dependent,
                    principal);
            }
        }
    }
}