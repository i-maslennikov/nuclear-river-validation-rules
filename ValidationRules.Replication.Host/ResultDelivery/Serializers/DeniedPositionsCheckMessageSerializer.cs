using System.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class DeniedPositionsCheckMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public DeniedPositionsCheckMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public int MessageType
            => DeniedPositionsCheckActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var positions = message.ReadOrderPositions();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"{MakePositionText(positions.First())} является запрещённой для: {MakePositionText(positions.Last())}" +
                                            $" в заказе {_linkFactory.CreateLink("Order", positions.Last().OrderId, positions.Last().OrderNumber)}");
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}