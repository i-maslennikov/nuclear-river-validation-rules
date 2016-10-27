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
            throw new NotSupportedException("Сообщение не привязано к заказу");
        }
    }
}