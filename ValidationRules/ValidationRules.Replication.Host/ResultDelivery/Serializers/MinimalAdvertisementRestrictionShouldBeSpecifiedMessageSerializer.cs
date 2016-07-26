using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => MinimalAdvertisementRestrictionShouldBeSpecifiedActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var projectReference = message.ReadProjectReference();
            var pricePositionName = message.ReadAttribute("pricePosition", "name");

            return new LocalizedMessage(Result.Error,
                                        $"Проект {_linkFactory.CreateLink(projectReference)}",
                                        $"В позиции прайса {pricePositionName} необходимо указать минимальное количество рекламы в выпуск");
        }
    }
}