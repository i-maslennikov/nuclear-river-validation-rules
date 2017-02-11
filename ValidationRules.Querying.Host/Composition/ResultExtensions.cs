using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public static class ResultExtensions
    {
        public static EntityReference ReadOrderReference(this Version.ValidationResult message)
            => ReadReference(message, "Order");

        public static EntityReference ReadOrderPositionReference(this Version.ValidationResult message)
            => ReadReference(message, "OrderPosition");

        public static IReadOnlyCollection<EntityReference> ReadOrderPositionReferences(this Version.ValidationResult message)
            => ReadReferences(message, "OrderPosition");

        public static EntityReference ReadPositionReference(this Version.ValidationResult message)
            => ReadReference(message, "Position");

        public static EntityReference ReadAdvertisementReference(this Version.ValidationResult message)
            => ReadReference(message, "Advertisement");

        public static EntityReference ReadAdvertisementElementReference(this Version.ValidationResult message)
            => ReadReference(message, "AdvertisementElement");

        public static EntityReference ReadProjectReference(this Version.ValidationResult message)
            => ReadReference(message, "Project");

        public static EntityReference ReadPricePositionReference(this Version.ValidationResult message)
            => ReadReference(message, "PricePosition");

        public static EntityReference ReadThemeReference(this Version.ValidationResult message)
            => ReadReference(message, "Theme");

        public static EntityReference ReadCategoryReference(this Version.ValidationResult message)
            => ReadReference(message, "Category");

        public static EntityReference ReadFirmAddressReference(this Version.ValidationResult message)
            => ReadReference(message, "FirmAddress");

        public static EntityReference ReadFirmReference(this Version.ValidationResult message)
            => ReadReference(message, "Firm");

        public static EntityReference ReadLegalPersonProfileReference(this Version.ValidationResult message)
            => ReadReference(message, "LegalPersonProfile");

        public static EntityReference ReadPriceReference(this Version.ValidationResult message)
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

            return new EntityReference("Price", (long)priceElement.Attribute("id"), $"{projectElement.Attribute("name")} от {priceElement.Attribute("beginDate")}");
        }

        public static AccountBalanceMessageDto ReadAccountBalanceMessage(this Version.ValidationResult message)
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
            };
        }

        public static IReadOnlyCollection<OrderPositionDto> ReadOrderPositions(this Version.ValidationResult message)
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

        public static AdvertisementCountDto ReadAdvertisementCountMessage(this Version.ValidationResult message)
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

        public static OversalesDto ReadOversalesMessage(this Version.ValidationResult message)
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

        public static InvalidFirmAddressState ReadFirmAddressState(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (InvalidFirmAddressState)(int)element.Attribute("invalidFirmAddressState");
        }

        public static CategoryCountDto ReadCategoryCount(this Version.ValidationResult message)
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

        public static InvalidFirmState ReadFirmState(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (InvalidFirmState)(int)element.Attribute("invalidFirmState");
        }

        public static Advertisement.ReviewStatus ReadAdvertisementElementStatus(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (Advertisement.ReviewStatus)(int)element.Attribute("advertisementElementStatus");
        }

        public static OrderRequiredFieldsDto ReadOrderRequiredFieldsMessage(this Version.ValidationResult message)
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

        public static OrderInactiveFieldsDto ReadOrderInactiveFieldsMessage(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return new OrderInactiveFieldsDto
            {
                LegalPerson = element.Element("legalPerson") != null,
                LegalPersonProfile = element.Element("legalPersonProfile") != null,
                BranchOfficeOrganizationUnit = element.Element("branchOfficeOrganizationUnit") != null,
                BranchOffice = element.Element("branchOffice") != null,
            };
        }

        public static int ReadProjectThemeCount(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (int)element.Attribute("themeCount");
        }

        public static string ReadWebsite(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return element.Attribute("website").Value;
        }

        public static DealState ReadDealState(this Version.ValidationResult message)
        {
            var element = message.MessageParams.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (DealState)Enum.Parse(typeof(DealState), element.Attribute("state").Value, true);
        }

        private static EntityReference ReadReference(Version.ValidationResult message, string type)
        {
            var elementName = MakeFirstLowercase(type);
            var element = message.MessageParams.Root.Element(elementName);
            if (element == null)
            {
                throw new ArgumentException($"Сообщение не содержит ссылку на '{elementName}'", nameof(message));
            }

            return new EntityReference(type, (long)element.Attribute("id"), (string)element.Attribute("name"));
        }

        private static IReadOnlyCollection<EntityReference> ReadReferences(Version.ValidationResult message, string type)
        {
            var elementName = MakeFirstLowercase(type);
            var elements = message.MessageParams.Root.Elements(elementName);
            return elements.Select(x => new EntityReference(type, (long)x.Attribute("id"), (string)x.Attribute("name"))).ToArray();
        }

        private static string MakeFirstLowercase(string s)
        {
            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        public enum DealState { Missing, Inactive }

        public sealed class CategoryCountDto
        {
            public int Allowed { get; set; }
            public int Actual { get; set; }
        }

        public sealed class AccountBalanceMessageDto
        {
            public decimal Available { get; set; }
            public decimal Planned { get; set; }
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

        public sealed class OrderInactiveFieldsDto
        {
            public bool LegalPerson { get; set; }
            public bool LegalPersonProfile { get; set; }
            public bool BranchOfficeOrganizationUnit { get; set; }
            public bool BranchOffice { get; set; }
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