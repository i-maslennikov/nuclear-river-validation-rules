namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LegalPersonProfileBargainShouldNotBeExpiredMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LegalPersonProfileBargainShouldNotBeExpiredMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var legalPersonProfileReference = message.ReadLegalPersonProfileReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"У юр. лица клиента, в профиле {legalPersonProfileReference} указан договор с датой окончания действия раньше даты подписания заказа");
        }
    }
}