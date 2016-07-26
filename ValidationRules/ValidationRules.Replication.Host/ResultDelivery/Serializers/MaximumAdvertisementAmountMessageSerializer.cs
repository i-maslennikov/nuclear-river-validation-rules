using System;

using NuClear.ValidationRules.Replication.PriceRules.Validation;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class MaximumAdvertisementAmountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory = new LinkFactory();

        public int MessageType
            => MaximumAdvertisementAmountActor.MessageTypeId;

        public LocalizedMessage Serialize(Message message)
        {
            var orderId = (long)message.Data.Root.Element("order").Attribute("id");
            var orderNumber = (string)message.Data.Root.Element("order").Attribute("number");
            var max = (int)message.Data.Root.Element("message").Attribute("max");
            var count = (int)message.Data.Root.Element("message").Attribute("count");
            var name = (string)message.Data.Root.Element("message").Attribute("name");
            var month = (DateTime)message.Data.Root.Element("message").Attribute("month");

            return new LocalizedMessage(Result.Error,
                                        $"Заказ {_linkFactory.CreateLink("Order", orderId, orderNumber)}",
                                        $"Позиция {name} должна присутствовать в сборке в количестве до {max}. Фактическое количество позиций в месяц {month:d} - {count}");
        }
    }
}