using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustUseCategoriesOnlyAvailableInProjectMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var categoryReference = references.Get<EntityTypeCategory>();
            var orderPositionReference = references.Get<EntityTypeOrderPositionAdvertisement>();

            return new MessageComposerResult(
                orderReference,
                Resources.OrdersCheckOrderPositionContainsCategoriesFromWrongOrganizationUnit,
                categoryReference,
                orderPositionReference);
        }
    }
}