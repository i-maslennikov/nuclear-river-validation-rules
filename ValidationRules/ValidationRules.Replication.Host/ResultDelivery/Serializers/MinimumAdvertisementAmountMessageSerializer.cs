using System;
using System.Xml.Linq;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class MinimumAdvertisementAmountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => MinimumAdvertisementAmountActor.MessageTypeId;

        public LocalizedMessage Serialize(XDocument document)
        {
            var orderId = (long)document.Root.Element("order").Attribute("id");
            var orderNumber = (string)document.Root.Element("order").Attribute("number");
            var min = (int)document.Root.Element("message").Attribute("min");
            var count = (int)document.Root.Element("message").Attribute("count");
            var name = (string)document.Root.Element("message").Attribute("name");
            var month = (DateTime)document.Root.Element("message").Attribute("month");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Позиция {name} должна присутствовать в сборке в количестве от {min}. Фактическое количество позиций в месяц {month:d} - {count}");
        }
    }
}