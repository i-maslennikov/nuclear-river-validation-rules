using System;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public static class ResultExtensions
    {
        public static AccountBalanceMessageDto ReadAccountBalanceMessage(this Message message)
        {
            var element = message.Xml.Root.Element("message");
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

        public static AdvertisementCountDto ReadAdvertisementCountMessage(this Message message)
        {
            var element = message.Xml.Root.Element("message");
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

        public static OversalesDto ReadOversalesMessage(this Message message)
        {
            var element = message.Xml.Root.Element("message");
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

        public static InvalidFirmAddressState ReadFirmAddressState(this Message message)
        {
            var element = message.Xml.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (InvalidFirmAddressState)(int)element.Attribute("invalidFirmAddressState");
        }

        public static CategoryCountDto ReadCategoryCount(this Message message)
        {
            var element = message.Xml.Root.Element("message");
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

        public static InvalidFirmState ReadFirmState(this Message message)
        {
            var element = message.Xml.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (InvalidFirmState)(int)element.Attribute("invalidFirmState");
        }

        public static Advertisement.ReviewStatus ReadAdvertisementElementStatus(this Message message)
        {
            var element = message.Xml.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (Advertisement.ReviewStatus)(int)element.Attribute("advertisementElementStatus");
        }

        public static OrderRequiredFieldsDto ReadOrderRequiredFieldsMessage(this Message message)
        {
            var element = message.Xml.Root.Element("message");
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

        public static OrderInactiveFieldsDto ReadOrderInactiveFieldsMessage(this Message message)
        {
            var element = message.Xml.Root.Element("message");
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

        public static int ReadProjectThemeCount(this Message message)
        {
            var element = message.Xml.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (int)element.Attribute("themeCount");
        }

        public static string ReadWebsite(this Message message)
        {
            var element = message.Xml.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return element.Attribute("website").Value;
        }

        public static DealState ReadDealState(this Message message)
        {
            var element = message.Xml.Root.Element("message");
            if (element == null)
            {
                throw new ArgumentException("Сообщение не содержит сообщения", nameof(message));
            }

            return (DealState)Enum.Parse(typeof(DealState), element.Attribute("state").Value, true);
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