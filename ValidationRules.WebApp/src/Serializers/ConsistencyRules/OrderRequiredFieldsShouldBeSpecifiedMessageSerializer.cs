using System.Collections.Generic;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class OrderRequiredFieldsShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderRequiredFieldsShouldBeSpecifiedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderRequiredFieldsShouldBeSpecified;

        public MessageTemplate Serialize(ValidationResult validationResult)
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
                parameters.Add("план");
            }
            if (dto.Currency)
            {
                parameters.Add("Валюта");
            }

            return new MessageTemplate(
                orderReference,
                "Необходимо заполнить все обязательные для заполнения поля: {0}",
                string.Join(", ", parameters));
        }
    }
}