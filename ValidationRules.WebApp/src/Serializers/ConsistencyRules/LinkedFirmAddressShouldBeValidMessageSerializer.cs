using System.Collections.Generic;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class LinkedFirmAddressShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedFirmAddressShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmAddressShouldBeValid;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var firmAddressReference = validationResult.ReadFirmAddressReference();

            var firmAddressState = validationResult.ReadFirmAddressState();
            var format = new Dictionary<InvalidFirmAddressState, string>
                {
                    { InvalidFirmAddressState.Deleted, "скрыт навсегда" },
                    { InvalidFirmAddressState.NotActive, "неактивен" },
                    { InvalidFirmAddressState.ClosedForAscertainment, "скрыт до выяснения" },
                    { InvalidFirmAddressState.NotBelongToFirm, "не принадлежит фирме заказа" }
                };

            return new MessageTemplate(
                orderReference,
                "В позиции {0} адрес фирмы {1} {2}",
                _linkFactory.CreateLink(orderPositionReference),
                _linkFactory.CreateLink(firmAddressReference),
                format[firmAddressState]);
        }
    }
}