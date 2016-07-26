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
            var priceId = (long)message.Data.Root.Element("price").Attribute("id");
            var priceBeginDate = (string)message.Data.Root.Element("price").Attribute("beginDate");
            var projectName = (string)message.Data.Root.Element("project").Attribute("name");
            var pricePositionName = (decimal)message.Data.Root.Element("pricePosition").Attribute("name");

            // todo: Вывести название прайс-листа из названия города и даты публикации. Думаю, это задача ui, т.е. должна решаться здесь.
            return new LocalizedMessage(Result.Error,
                                        $"Прайс-лист {_linkFactory.CreateLink("Price", priceId, $"{projectName} от {priceBeginDate}")}",
                                        $"В Позиции прайс-листа {pricePositionName} содержится более одной группы сопутствующих позиций, что не поддерживается системой");
        }
    }
}