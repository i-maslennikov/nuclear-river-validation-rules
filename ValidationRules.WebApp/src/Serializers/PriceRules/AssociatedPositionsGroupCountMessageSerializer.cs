using System;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class AssociatedPositionsGroupCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AssociatedPositionsGroupCountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionsGroupCount;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var price = validationResult.ReadPriceReference();
            var pricePosition = validationResult.ReadPricePositionReference();

            return new MessageTemplate(
                price,
                "В Позиции прайс-листа {0} содержится более одной группы сопутствующих позиций, что не поддерживается системой",
                _linkFactory.CreateLink(pricePosition));
        }
    }
}