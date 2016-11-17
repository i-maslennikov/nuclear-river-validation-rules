using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

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
                parameters.Add("Юр. лицо заказчика");
            }
            if (dto.LegalPersonProfile)
            {
                parameters.Add("Профиль юр. лица заказчика");
            }
            if (dto.BranchOfficeOrganizationUnit)
            {
                parameters.Add("Юр. лицо исполнителя");
            }
            if (dto.BranchOffice)
            {
                parameters.Add("Юр. лицо организации");
            }

            return new MessageComposerResult(
                orderReference,
                $"Заказ ссылается на неактивные объекты: {string.Join(", ", parameters)}");
        }
    }
}