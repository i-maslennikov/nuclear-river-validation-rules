using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class DeniedPositionsCheckMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => DeniedPositionsCheckActor.MessageTypeId;

        public LocalizedMessage Serialize(XDocument document)
        {
            var orderId = (long)document.Root.Element("order").Attribute("id");
            var orderNumber = (string)document.Root.Element("order").Attribute("number");
            var positions = document.Root.Elements("position").Select(ParseOrderPosition).ToArray();

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"{MakePositionText(positions[0])} является запрещённой для: {MakePositionText(positions[1])}" +
                                            $" в заказе {_linkFactory.CreateLink("Order", positions[1].OrderId, positions[1].OrderNumber)}");
        }

        private OrderPositionDto ParseOrderPosition(XElement element)
        {
            return new OrderPositionDto
                {
                    OrderId = (long)element.Attribute("orderId"),
                    OrderNumber = (string)element.Attribute("orderNumber"),
                    OrderPositionId = (long)element.Attribute("orderPositionId"),
                    OrderPositionName = (string)element.Attribute("orderPositionName"),
                    PositionId = (long)element.Attribute("positionId"),
                    PositionName = (string)element.Attribute("positionName"),
                };
        }

        private string MakePositionText(OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }

        private class OrderPositionDto
        {
            public long OrderId { get; set; }
            public string OrderNumber { get; set; }
            public long OrderPositionId { get; set; }
            public string OrderPositionName { get; set; }
            public long PositionId { get; set; }
            public string PositionName { get; set; }
        }
    }
}