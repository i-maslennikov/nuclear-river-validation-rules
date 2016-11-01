using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;
using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public static class MessageExtensions
    {
        public static Result GetLevel(this ValidationResult message)
            => new ResultBuilder(message.Result).WhenSingle();

        public static string ReadAttribute(this ValidationResult message, string elementName, string attributeName)
        {
            var element = message.MessageParams.Root.Element(elementName);
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

        public static Tuple<string, long, string> ReadOrderReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("order");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на заказ", nameof(message));
            }

            return Tuple.Create("Order", (long)element.Attribute("id"), (string)element.Attribute("number"));
        }

        public static Tuple<string, long, string> ReadOrderPositionReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("orderPosition");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на позицию заказа", nameof(message));
            }

            return Tuple.Create("OrderPosition", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static IReadOnlyCollection<Tuple<string, long, string>> ReadOrderPositionReferences(this ValidationResult message)
        {
            var elements = message.MessageParams.Root.Elements("orderPosition");
            return elements
                .Select(x => Tuple.Create("OrderPosition", (long)x.Attribute("id"), (string)x.Attribute("name")))
                .ToArray();
        }

        public static Tuple<string, long, string> ReadPositionReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("position");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на позицию номенклатуры", nameof(message));
            }

            return Tuple.Create("Position", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadAdvertisementReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("advertisement");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на позицию номенклатуры", nameof(message));
            }

            return Tuple.Create("Advertisement", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadAdvertisementElementReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("advertisementElement");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на ЭРМ", nameof(message));
            }

            return Tuple.Create("AdvertisementElement", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadProjectReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("project");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на проект", nameof(message));
            }

            return Tuple.Create("Project", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadPriceReference(this ValidationResult message)
        {
            var priceElement = message.MessageParams.Root.Element("price");
            if (priceElement == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на прайс-лист", nameof(message));
            }

            var projectElement = message.MessageParams.Root.Element("project");
            if (projectElement == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на проект", nameof(message));
            }

            return Tuple.Create("Price", (long)priceElement.Attribute("id"), $"{projectElement.Attribute("name")} от {priceElement.Attribute("beginDate")}");
        }

        public static Tuple<string, long, string> ReadPricePositionReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("pricePosition");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на позицию прайс-листа", nameof(message));
            }

            return Tuple.Create("PricePosition", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadThemeReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("theme");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на тематику", nameof(message));
            }

            return Tuple.Create("Theme", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadCategoryReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("category");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на рубрику", nameof(message));
            }

            return Tuple.Create("Category", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadFirmAddressReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("firmAddress");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на адрес", nameof(message));
            }

            return Tuple.Create("FirmAddress", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadFirmReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("firm");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на фирму", nameof(message));
            }

            return Tuple.Create("Firm", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static Tuple<string, long, string> ReadLegalPersonProfileReference(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("legalPersonProfile");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит ссылки на профиль юр. лица клиента", nameof(message));
            }

            return Tuple.Create("LegalPersonProfile", (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        public static AccountBalanceMessageDto ReadAccountBalanceMessage(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
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

        public static IReadOnlyCollection<OrderPositionDto> ReadOrderPositions(this ValidationResult message)
        {
            return message.MessageParams.Root.Elements("position").Select(element => new OrderPositionDto
            {
                OrderId = (long)element.Attribute("orderId"),
                OrderNumber = (string)element.Attribute("orderNumber"),
                OrderPositionId = (long)element.Attribute("orderPositionId"),
                OrderPositionName = (string)element.Attribute("orderPositionName"),
                PositionId = (long)element.Attribute("positionId"),
                PositionName = (string)element.Attribute("positionName"),
            }).ToArray();
        }

        public static AdvertisementCountDto ReadAdvertisementCountMessage(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
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

        public static OversalesDto ReadOversalesMessage(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return new OversalesDto
            {
                Max = (int)element.Attribute("max"),
                Count = (int)element.Attribute("count"),
            };
        }

        public static InvalidFirmAddressState ReadFirmAddressState(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (InvalidFirmAddressState)(int)element.Attribute("invalidFirmAddressState");
        }

        public static CategoryCountDto ReadCategoryCount(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return new CategoryCountDto
            {
                Actual = (int)element.Attribute("count"),
                Allowed = (int)element.Attribute("allowed")
            };
        }

        public static InvalidFirmState ReadFirmState(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (InvalidFirmState)(int)element.Attribute("invalidFirmState");
        }

        public static ReviewStatus ReadAdvertisementElementStatus(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (ReviewStatus)(int)element.Attribute("advertisementElementStatus");
        }

        public static OrderRequiredFieldsDto ReadOrderRequiredFieldsMessage(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return new OrderRequiredFieldsDto
            {
                LegalPerson = element.Element("legalPerson") != null,
                LegalPersonProfile = element.Element("legalPersonProfile") != null,
                BranchOfficeOrganizationUnit = element.Element("branchOfficeOrganizationUnit") != null,
                Inspector = element.Element("inspector") != null,
                ReleaseCountPlan = element.Element("releaseCountPlan") != null,
                Currency = element.Element("currency") != null,
            };
        }

        public static int ReadProjectThemeCount(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (int)element.Attribute("themeCount");
        }

        public static string ReadWebsite(this ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return element.Attribute("website").Value;
        }

        public sealed class CategoryCountDto
        {
            public int Allowed { get; set; }
            public int Actual { get; set; }
        }

        public sealed class AccountBalanceMessageDto
        {
            public decimal Available { get; set; }
            public decimal Planned { get; set; }
            public decimal Required { get; set; }
        }

        public sealed class OrderPositionDto
        {
            public long OrderId { get; set; }
            public string OrderNumber { get; set; }
            public long OrderPositionId { get; set; }
            public string OrderPositionName { get; set; }
            public long PositionId { get; set; }
            public string PositionName { get; set; }
        }

        public sealed class AdvertisementCountDto
        {
            public int Min { get; set; }
            public int Max { get; set; }
            public int Count { get; set; }
            public string Name { get; set; }
            public DateTime Month { get; set; }
        }

        public sealed class OversalesDto
        {
            public int Max { get; set; }
            public int Count { get; set; }
        }

        public sealed class OrderRequiredFieldsDto
        {
            public bool LegalPerson { get; set; }
            public bool LegalPersonProfile { get; set; }
            public bool BranchOfficeOrganizationUnit { get; set; }
            public bool Inspector { get; set; }
            public bool ReleaseCountPlan { get; set; }
            public bool Currency { get; set; }
        }
    }
}