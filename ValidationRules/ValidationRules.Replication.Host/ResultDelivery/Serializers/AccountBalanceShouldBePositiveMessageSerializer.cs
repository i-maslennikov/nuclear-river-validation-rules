using System.Xml.Linq;

using NuClear.ValidationRules.Replication.AccountRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AccountBalanceShouldBePositiveMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => AccountBalanceShouldBePositiveActor.MessageTypeId;

        public LocalizedMessage Serialize(XDocument document)
        {
            var orderId = (long)document.Root.Element("order").Attribute("id");
            var orderNumber = (string)document.Root.Element("order").Attribute("number");
            var available = (decimal)document.Root.Element("message").Attribute("available");
            var planned = (decimal)document.Root.Element("message").Attribute("planned");
            var required = (decimal)document.Root.Element("message").Attribute("required");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Для оформления заказа недостаточно средств. Необходимо: {planned}. Имеется: {available}. Необходим лимит: {required}");
        }
    }
}