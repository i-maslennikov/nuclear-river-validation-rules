using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public static class ResultExtensions
    {
        public static AccountBalanceMessageDto ReadAccountBalanceMessage(this IReadOnlyDictionary<string, string> message)
        {
            return new AccountBalanceMessageDto
                {
                    Available = decimal.Parse(message["available"]),
                    Planned = decimal.Parse(message["planned"]),
                };
        }

        public static AdvertisementCountDto ReadAdvertisementCountMessage(this IReadOnlyDictionary<string, string> message)
        {
            return new AdvertisementCountDto
            {
                Min = int.Parse(message["min"]),
                Max = int.Parse(message["max"]),
                Count = int.Parse(message["count"]),
                Name = message["name"],
                Month = DateTime.Parse(message["month"]),
            };
        }

        public static OversalesDto ReadOversalesMessage(this IReadOnlyDictionary<string, string> message)
        {
            return new OversalesDto
                {
                    Max = int.Parse(message["max"]),
                    Count = int.Parse(message["count"]),
                };
        }

        public static InvalidFirmAddressState ReadFirmAddressState(this IReadOnlyDictionary<string, string> message)
        {
            return (InvalidFirmAddressState)int.Parse(message["invalidFirmAddressState"]);
        }

        public static CategoryCountDto ReadCategoryCount(this IReadOnlyDictionary<string, string> message)
        {
            return new CategoryCountDto
            {
                Actual = int.Parse(message["count"]),
                Allowed = int.Parse(message["allowed"]),
            };
        }

        public static InvalidFirmState ReadFirmState(this IReadOnlyDictionary<string, string> message)
        {
            return (InvalidFirmState)int.Parse(message["invalidFirmState"]);
        }

        public static Advertisement.ReviewStatus ReadAdvertisementElementStatus(this IReadOnlyDictionary<string, string> message)
        {
            return (Advertisement.ReviewStatus)int.Parse(message["advertisementElementStatus"]);
        }

        public static OrderRequiredFieldsDto ReadOrderRequiredFieldsMessage(this IReadOnlyDictionary<string, string> message)
        {
            return new OrderRequiredFieldsDto
            {
                LegalPerson = bool.Parse(message["legalPerson"]),
                LegalPersonProfile = bool.Parse(message["legalPersonProfile"]),
                BranchOfficeOrganizationUnit = bool.Parse(message["legalPerson"]),
                Inspector = bool.Parse(message["inspector"]),
                ReleaseCountPlan = bool.Parse(message["releaseCountPlan"]),
                Currency = bool.Parse(message["currency"]),
            };
        }

        public static OrderInactiveFieldsDto ReadOrderInactiveFieldsMessage(this IReadOnlyDictionary<string, string> message)
        {
            return new OrderInactiveFieldsDto
            {
                LegalPerson = bool.Parse(message["legalPerson"]),
                LegalPersonProfile = bool.Parse(message["legalPersonProfile"]),
                BranchOfficeOrganizationUnit = bool.Parse(message["branchOfficeOrganizationUnit"]),
                BranchOffice = bool.Parse(message["branchOffice"]),
            };
        }

        public static int ReadProjectThemeCount(this IReadOnlyDictionary<string, string> message)
        {
            return int.Parse(message["themeCount"]);
        }

        public static string ReadWebsite(this IReadOnlyDictionary<string, string> message)
        {
            return message["website"];
        }

        public static DealState ReadDealState(this IReadOnlyDictionary<string, string> message)
        {
            return (DealState)int.Parse(message["state"]);
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