using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustUseCategoriesOnlyAvailableInProjectMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var categoryReference = references.Get("category");
            var orderPositionReference = references.Get("orderPosition");

            return new MessageComposerResult(
                orderReference,
                Resources.OrdersCheckOrderPositionContainsCategoriesFromWrongOrganizationUnit,
                categoryReference,
                orderPositionReference);
        }
    }
}