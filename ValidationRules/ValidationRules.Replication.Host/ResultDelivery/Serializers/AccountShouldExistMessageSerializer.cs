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
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Заказ не имеет привязки к лицевому счёту");
        }
    }
}