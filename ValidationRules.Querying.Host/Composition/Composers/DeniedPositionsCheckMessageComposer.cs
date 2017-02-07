using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class DeniedPositionsCheckMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DeniedPositionsCheck;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var dependentOrder = references.GetMany("order").First();
            var principalOrder = references.GetMany("order").Last();

            var orderPositions = references.GetMany("orderPosition").ToList();
            var dependentOrderPosition = orderPositions[0];
            var dependentPosition = orderPositions[1];
            var principalOrderPosition = orderPositions[2];
            var principalPosition = orderPositions[3];

            if (dependentOrder.Id != principalOrder.Id)
            {
                return new MessageComposerResult(
                    dependentOrder,
                    string.Format(
                        Resources.ADPCheckModeSpecificOrder_MessageTemplate + Resources.OrderDescriptionTemplate,
                        MakePositionText(dependentOrderPosition, dependentPosition),
                        MakePositionText(principalOrderPosition, principalPosition)),
                    dependentOrderPosition,
                    principalOrderPosition,
                    principalOrder);
            }
            else
            {
                return new MessageComposerResult(
                    dependentOrder,
                    string.Format(
                        Resources.ADPCheckModeSpecificOrder_MessageTemplate,
                        MakePositionText(dependentOrderPosition, dependentPosition),
                        MakePositionText(principalOrderPosition, principalPosition)),
                    dependentOrderPosition,
                    principalOrderPosition);
            }
        }

        private static string MakePositionText(EntityReference orderPosition, EntityReference position)
        {
            return orderPosition.Name != position.Name
                       ? string.Format(Resources.RichChildPositionTypeTemplate, position.Name)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}