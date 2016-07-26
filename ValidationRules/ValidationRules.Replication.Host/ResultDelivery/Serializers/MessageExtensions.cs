using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public static class MessageExtensions
    {
        public static string ReadAttribute(this Message message, string elementName, string attributeName)
        {
            var element = message.Data.Root.Element(elementName);
            if (element == null)
            {
                throw new ArgumentException($"Сообщение не содержит элемент {elementName}", nameof(message));
            }

            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                throw new ArgumentException($"Элемент {elementName} сообщения не содержит аттрибут {attributeName}", nameof(message));
            }

            return attribute.Value;
        }

        public static Tuple<string, long, string> ReadOrderReference(this Message message)
        {
            var element = message.Data.Root.Element("order");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на заказ", nameof(message));
            }

            return Tuple.Create("Order", (long)element.Attribute("id"), (string)element.Attribute("number"));
        }

        public static Tuple<string, long, string> ReadOrderPositionReference(this Message message)
        {
            var element = message.Data.Root.Element("orderPosition");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на позицию заказа", nameof(message));
            }

            return Tuple.Create("OrderPosition", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadProjectReference(this Message message)
        {
            var element = message.Data.Root.Element("project");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на проект", nameof(message));
            }

            return Tuple.Create("Project", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadPriceReference(this Message message)
        {
            var priceElement = message.Data.Root.Element("price");
            if (priceElement == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на прайс-лист", nameof(message));
            }

            var projectElement = message.Data.Root.Element("project");
            if (projectElement == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на проект", nameof(message));
            }

            return Tuple.Create("Price", (long)priceElement.Attribute("id"), $"{projectElement.Attribute("name")} от {priceElement.Attribute("beginDate")}");
        }

        public static Tuple<string, long, string> ReadPricePositionReference(this Message message)
        {
            var element = message.Data.Root.Element("pricePosition");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на позицию прайс-листа", nameof(message));
            }

            return Tuple.Create("PricePosition", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static AccountBalanceMessageDto ReadAccountBalanceMessage(this Message message)
        {
            var element = message.Data.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return new AccountBalanceMessageDto
                {
                    Available = (decimal)element.Attribute("available"),
                    Planned = (decimal)element.Attribute("planned"),
                    Required = (decimal)element.Attribute("required"),
                };
        }

        public static IReadOnlyCollection<OrderPositionDto> ReadOrderPositions(this Message message)
        {
            return message.Data.Root.Elements("position").Select(element => new OrderPositionDto
                {
                    OrderId = (long)element.Attribute("orderId"),
                    OrderNumber = (string)element.Attribute("orderNumber"),
                    OrderPositionId = (long)element.Attribute("orderPositionId"),
                    OrderPositionName = (string)element.Attribute("orderPositionName"),
                    PositionId = (long)element.Attribute("positionId"),
                    PositionName = (string)element.Attribute("positionName"),
                }).ToArray();
        }

        public static AdvertisementCountDto ReadAdvertisementCountMessage(this Message message)
        {
            var element = message.Data.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return new AdvertisementCountDto
                {
                    Min = (int)element.Attribute("min"),
                    Max = (int)element.Attribute("max"),
                    Count = (int)element.Attribute("count"),
                    Name = (string)element.Attribute("name"),
                    Month = (DateTime)element.Attribute("month"),
                };
        }

        public class AccountBalanceMessageDto
        {
            public decimal Available { get; set; }
            public decimal Planned { get; set; }
            public decimal Required { get; set; }
        }

        public class OrderPositionDto
        {
            public long OrderId { get; set; }
            public string OrderNumber { get; set; }
            public long OrderPositionId { get; set; }
            public string OrderPositionName { get; set; }
            public long PositionId { get; set; }
            public string PositionName { get; set; }
        }

        public class AdvertisementCountDto
        {
            public int Min { get; set; }
            public int Max { get; set; }
            public int Count { get; set; }
            public string Name { get; set; }
            public DateTime Month { get; set; }
        }
    }
}