using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class OrderPositionsShouldCorrespontToActualPriceMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => OrderPositionsShouldCorrespontToActualPriceActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderId = (long)message.Data.Root.Element("order").Attribute("id");
            var orderNumber = (string)message.Data.Root.Element("order").Attribute("number");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Не найден действующий для заказа прайс-лист (в erm текст другой)");
        }
    }
}