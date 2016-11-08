using System;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var price = validationResult.ReadPriceReference();
            var pricePosition = validationResult.ReadPricePositionReference();

            return new MessageTemplate(
                price,
                "В позиции прайса {0} необходимо указать минимальное количество рекламы в выпуск",
                _linkFactory.CreateLink(pricePosition));
        }
    }
}