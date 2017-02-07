using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderRequiredFieldsShouldBeSpecifiedMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderRequiredFieldsShouldBeSpecified;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var dto = message.ReadOrderRequiredFieldsMessage();

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
            if (dto.Inspector)
            {
                parameters.Add(Resources.Inspector);
            }
            if (dto.ReleaseCountPlan)
            {
                parameters.Add(Resources.PlanReleaseCount);
            }
            if (dto.Currency)
            {
                parameters.Add(Resources.Currency);
            }

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.OrderCheckOrderHasUnspecifiedFields, string.Join(", ", parameters)));
        }
    }
}