using NuClear.ValidationRules.Replication.AccountRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AccountShouldExistMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => AccountShouldExistActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderId = (long)message.Data.Root.Element("order").Attribute("id");
            var orderNumber = (string)message.Data.Root.Element("order").Attribute("number");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Заказ не имеет привязки к лицевому счёту");
        }
    }
}