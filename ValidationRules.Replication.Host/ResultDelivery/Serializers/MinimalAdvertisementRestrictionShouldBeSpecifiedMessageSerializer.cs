namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public int MessageType => 2;

        public LocalizedMessage Serialize(Message message)
        {
            var projectReference = message.ReadProjectReference();
            var pricePositionName = message.ReadAttribute("pricePosition", "name");

            return new LocalizedMessage(message.GetLevel(),
                                        $"Проект {_linkFactory.CreateLink(projectReference)}",
                                        $"В позиции прайса {pricePositionName} необходимо указать минимальное количество рекламы в выпуск");
        }
    }
}