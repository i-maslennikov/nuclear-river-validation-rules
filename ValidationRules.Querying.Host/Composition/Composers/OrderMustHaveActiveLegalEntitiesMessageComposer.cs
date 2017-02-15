using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveActiveLegalEntitiesMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveLegalEntities;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var dto = extra.ReadOrderInactiveFieldsMessage();

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