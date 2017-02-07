using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedObjectsMissedInPrincipalsMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedObjectsMissedInPrincipals;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var orderPositionReference = references.GetMany("orderPosition").First();
            var positionReference = references.GetMany("orderPosition").Last();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.LinkedObjectsMissedInPrincipals, MakePositionText(orderPositionReference, positionReference)),
                orderPositionReference);
        }

        private static string MakePositionText(EntityReference orderPosition, EntityReference position)
        {
            return orderPosition.Name != position.Name
                       ? string.Format(Resources.RichChildPositionTypeTemplate, position.Name)
                       : Resources.RichDefaultPositionTypeTemplate;
        }
    }
}