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
            throw new NotSupportedException("Сообщение не привязано к заказу");
        }
    }
}