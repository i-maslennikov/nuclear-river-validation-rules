using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AssociatedPositionsGroupCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => AssociatedPositionsGroupCountActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var priceReference = message.ReadPriceReference();
            var pricePositionReference = message.ReadPricePositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Прайс-лист {_linkFactory.CreateLink(priceReference)}",
                                        $"В Позиции прайс-листа {_linkFactory.CreateLink(pricePositionReference)} содержится более одной группы сопутствующих позиций, что не поддерживается системой");
        }
    }
}