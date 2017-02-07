using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveActiveLegalEntitiesMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveLegalEntities;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var dto = message.ReadOrderInactiveFieldsMessage();

            var parameters = new List<string>();

            if (dto.LegalPerson)
            {
                parameters.Add(Resources.LegalPerson);
            }
            if (dto.LegalPersonProfile)
            {
                parameters.Add(Resources.LegalPersonProfile);
            }
            if (dto.BranchOfficeOrganizationUnit)
            {
                parameters.Add(Resources.BranchOfficeOrganizationUnit);
            }
            if (dto.BranchOffice)
            {
                parameters.Add(Resources.BranchOffice);
            }

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.OrderReferencesInactiveEntities, string.Join(", ", parameters)));
        }
    }
}