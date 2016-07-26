using System.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AssociatedPositionWithoutPrincipalMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => AssociatedPositionWithoutPrincipalActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var positions = message.ReadOrderPositions();

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"{MakePositionText(positions.First())} является сопутствующей, основная позиция не найдена");
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}