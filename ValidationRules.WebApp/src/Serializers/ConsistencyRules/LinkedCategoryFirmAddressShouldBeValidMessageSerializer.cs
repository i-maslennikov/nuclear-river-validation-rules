using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class LinkedCategoryFirmAddressShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedCategoryFirmAddressShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var firmAddressReference = validationResult.ReadFirmAddressReference();

            return new MessageTemplate(
                orderReference,
                "В позиции {0} найдена рубрика {1}, не принадлежащая адресу {2}",
                _linkFactory.CreateLink(orderPositionReference),
                _linkFactory.CreateLink(categoryReference),
                _linkFactory.CreateLink(firmAddressReference));
        }
    }
}