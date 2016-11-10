using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderRequiredFieldsShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderRequiredFieldsShouldBeSpecified;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadOrderRequiredFieldsMessage();

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
            if (dto.Inspector)
            {
                parameters.Add("Проверяющий");
            }
            if (dto.ReleaseCountPlan)
            {
                parameters.Add("План");
            }
            if (dto.Currency)
            {
                parameters.Add("Валюта");
            }

            return new MessageSerializerResult(
                orderReference,
                $"Необходимо заполнить все обязательные для заполнения поля: {string.Join(", ", parameters)}");
        }
    }
}