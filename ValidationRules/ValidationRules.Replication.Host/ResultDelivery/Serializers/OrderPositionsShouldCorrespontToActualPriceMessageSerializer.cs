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
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Не найден действующий для заказа прайс-лист");
        }
    }
}