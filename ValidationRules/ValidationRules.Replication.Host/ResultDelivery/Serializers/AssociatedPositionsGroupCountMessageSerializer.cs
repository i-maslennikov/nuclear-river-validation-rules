using System.Xml.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class AssociatedPositionsGroupCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => AssociatedPositionsGroupCountActor.MessageTypeId;

        public LocalizedMessage Serialize(XDocument document)
        {
            var priceId = (long)document.Root.Element("price").Attribute("id");
            var pricePublishDate = (string)document.Root.Element("price").Attribute("publishDate");
            var projectName = (string)document.Root.Element("project").Attribute("name");
            var pricePositionName = (decimal)document.Root.Element("pricePosition").Attribute("name");

            // todo: Вывести название прайс-листа из названия города и даты публикации. Думаю, это задача ui, т.е. должна решаться здесь.
            return new LocalizedMessage(Result.Error,
                                        $"Прайс-лист {_linkFactory.CreateLink("Price", priceId, $"{projectName} от {pricePublishDate}")}",
                                        $"В Позиции прайс-листа {pricePositionName} содержится более одной группы сопутствующих позиций, что не поддерживается системой");
        }
    }
}