using System.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => SatisfiedPrincipalPositionDifferentOrderActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositions = message.ReadOrderPositions();

            var masterPosition = $"{MakePositionText(orderPositions.First())} данного Заказа является основной для следующих позиций: ";
            var dependentPositions = orderPositions.Skip(1).Select(x => $"{MakePositionText(x)} Заказа {_linkFactory.CreateLink("Order", x.OrderId, x.OrderNumber)}");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        masterPosition + string.Join(", ", dependentPositions));
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}