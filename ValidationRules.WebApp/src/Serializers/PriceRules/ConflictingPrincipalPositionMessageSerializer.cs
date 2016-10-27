using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class ConflictingPrincipalPositionMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public ConflictingPrincipalPositionMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.ConflictingPrincipalPosition;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var positions = validationResult.ReadOrderPositions();

            return new MessageTemplate(
                orderReference,
                "{0} содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: {1}",
                MakePositionText(positions.First()),
                MakePositionText(positions.Last()));
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}