using System.Xml.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => MinimalAdvertisementRestrictionShouldBeSpecifiedActor.MessageTypeId;

        public LocalizedMessage Serialize(XDocument document)
        {
            var projectId = (long)document.Root.Element("project").Attribute("id");
            var projectName = (string)document.Root.Element("project").Attribute("name");
            var pricePositionName = (decimal)document.Root.Element("pricePosition").Attribute("name");

            return new LocalizedMessage(Result.Error,
                                        $"Проект {_linkFactory.CreateLink("Project", projectId, projectName)}",
                                        $"В позиции прайса {pricePositionName} необходимо указать минимальное количество рекламы в выпуск");
        }
    }
}