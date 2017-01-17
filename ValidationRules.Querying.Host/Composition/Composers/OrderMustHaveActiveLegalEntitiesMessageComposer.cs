using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveActiveLegalEntitiesMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveActiveLegalEntities;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadOrderInactiveFieldsMessage();

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