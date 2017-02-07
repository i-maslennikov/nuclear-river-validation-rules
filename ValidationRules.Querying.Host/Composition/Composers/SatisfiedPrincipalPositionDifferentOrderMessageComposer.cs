using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var principalOrder = references.GetMany("order").First();
            var dependentOrder = references.GetMany("order").Last();

            var orderPositions = references.GetMany("orderPosition").ToList();
            var principalOrderPosition = orderPositions[0];
            var principalPosition = orderPositions[1];
            var dependentOrderPosition = orderPositions[2];
            var dependentPosition = orderPositions[3];

            return new MessageComposerResult(
                principalOrder,
                string.Format(
                    Resources.ADPValidation_Template,
                    MakePositionText(principalOrderPosition, principalPosition),
                    MakePositionText(dependentOrderPosition, dependentPosition)),
                principalOrderPosition,
                dependentOrderPosition,
                dependentOrder);
        }

        private static string MakePositionText(EntityReference orderPosition, EntityReference position)
        {
            return orderPosition.Name != position.Name
                       ? string.Format(Resources.RichChildPositionTypeTemplate, position.Name)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}