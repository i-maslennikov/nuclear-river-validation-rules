using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class OrderPositionCorrespontToInactivePositionMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => OrderPositionCorrespontToInactivePositionActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderId = (long)message.Data.Root.Element("order").Attribute("id");
            var orderNumber = (string)message.Data.Root.Element("order").Attribute("number");
            var orderPositionId = (long)message.Data.Root.Element("orderPosition").Attribute("id");
            var orderPositionName = (string)message.Data.Root.Element("orderPosition").Attribute("name");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Позиция {_linkFactory.CreateLink("OrderPosition", orderPositionId, orderPositionName)} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа.");
        }
    }
}