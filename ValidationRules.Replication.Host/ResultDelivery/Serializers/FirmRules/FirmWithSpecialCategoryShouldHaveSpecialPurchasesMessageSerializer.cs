namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.FirmRules
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"У фирмы {_linkFactory.CreateLink(firmReference)}, с рубрикой \"Выгодные покупки с 2ГИС\", отсутствуют продажи по позициям \"Самореклама только для ПК\" или \"Выгодные покупки с 2ГИС\"");
        }
    }
}