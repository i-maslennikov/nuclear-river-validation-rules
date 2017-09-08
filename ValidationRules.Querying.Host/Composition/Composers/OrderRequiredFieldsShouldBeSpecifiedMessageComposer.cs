using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderRequiredFieldsShouldBeSpecifiedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderRequiredFieldsShouldBeSpecified;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var dto = extra.ReadOrderRequiredFieldsMessage();

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
            if (dto.Currency)
            {
                parameters.Add(Resources.Currency);
            }

            return new MessageComposerResult(
                orderReference,
                Resources.OrderCheckOrderHasUnspecifiedFields,
                string.Join(", ", parameters));
        }
    }
}