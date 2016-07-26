using NuClear.ValidationRules.Replication.AccountRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AccountBalanceShouldBePositiveMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => AccountBalanceShouldBePositiveActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderId = (long)message.Data.Root.Element("order").Attribute("id");
            var orderNumber = (string)message.Data.Root.Element("order").Attribute("number");
            var available = (decimal)message.Data.Root.Element("message").Attribute("available");
            var planned = (decimal)message.Data.Root.Element("message").Attribute("planned");
            var required = (decimal)message.Data.Root.Element("message").Attribute("required");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Для оформления заказа недостаточно средств. Необходимо: {planned}. Имеется: {available}. Необходим лимит: {required}");
        }
    }
}