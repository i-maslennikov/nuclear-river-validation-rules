using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderRequiredFieldsShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderRequiredFieldsShouldBeSpecifiedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderRequiredFieldsShouldBeSpecified;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var dto = message.ReadOrderRequiredFieldsMessage();

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

            return new LocalizedMessage(message.GetLevel(),
                                    $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                    "Необходимо заполнить все обязательные для заполнения поля: " + string.Join(", ", parameters));
        }
    }
}