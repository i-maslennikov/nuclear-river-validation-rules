using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class DeniedPositionsCheckMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public DeniedPositionsCheckMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.DeniedPositionsCheck;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var positions = validationResult.ReadOrderPositions().OrderBy(x => x.OrderPositionId);

            return new MessageTemplate(
                orderReference,
                "{0} является запрещённой для: {1} в заказе {2}",
                MakePositionText(positions.First()),
                MakePositionText(positions.Last()),
                _linkFactory.CreateLink("Order", positions.Last().OrderId, positions.Last().OrderNumber));
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}