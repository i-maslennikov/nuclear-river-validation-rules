using System.Xml.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class OrderPositionShouldCorrespontToActualPriceMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => OrderPositionShouldCorrespontToActualPriceActor.MessageTypeId;

        public LocalizedMessage Serialize(XDocument document)
        {
            var orderId = (long)document.Root.Element("order").Attribute("id");
            var orderNumber = (string)document.Root.Element("order").Attribute("number");
            var orderPositionId = (long)document.Root.Element("orderPosition").Attribute("id");
            var orderPositionName = (string)document.Root.Element("orderPosition").Attribute("name");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Позиция {_linkFactory.CreateLink("OrderPosition", orderPositionId, orderPositionName)} не соответствует актуальному прайс-листу. Необходимо указать позицию из текущего действующего прайс-листа");
        }
    }
}
