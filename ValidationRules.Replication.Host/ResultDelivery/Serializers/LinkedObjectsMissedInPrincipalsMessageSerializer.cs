using System.Linq;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class LinkedObjectsMissedInPrincipalsMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedObjectsMissedInPrincipalsMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public int MessageType => 10;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositions = message.ReadOrderPositions();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"{MakePositionText(orderPositions.First())} содержит объекты привязки, отсутствующие в основных позициях");
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}