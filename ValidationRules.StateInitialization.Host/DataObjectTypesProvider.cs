using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Messages = NuClear.ValidationRules.Storage.Model.Messages;

using WebApp = NuClear.ValidationRules.Storage.Model.WebApp;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public static readonly Type[] FactTypes =
            {
                typeof(Facts::Account),
                typeof(Facts::AssociatedPosition),
                typeof(Facts::AssociatedPositionsGroup),
                typeof(Facts::Bargain),
                typeof(Facts::BargainScanFile),
                typeof(Facts::Bill),
                typeof(Facts::BranchOffice),
                typeof(Facts::BranchOfficeOrganizationUnit),
                typeof(Facts::Category),
                typeof(Facts::CategoryOrganizationUnit),
                typeof(Facts::CostPerClickCategoryRestriction),
                typeof(Facts::Deal),
                typeof(Facts::DeniedPosition),
                typeof(Facts::EntityName),
                typeof(Facts::Firm),
                typeof(Facts::FirmAddress),
                typeof(Facts::FirmAddressCategory),
                typeof(Facts::LegalPerson),
                typeof(Facts::LegalPersonProfile),
                typeof(Facts::Lock),
                typeof(Facts::NomenclatureCategory),
                typeof(Facts::Order),
                typeof(Facts::OrderItem),
                typeof(Facts::OrderPosition),
                typeof(Facts::OrderPositionAdvertisement),
                typeof(Facts::OrderPositionCostPerClick),
                typeof(Facts::OrderScanFile),
                typeof(Facts::Position),
                typeof(Facts::PositionChild),
                typeof(Facts::Price),
                typeof(Facts::PricePosition),
                typeof(Facts::Project),
                typeof(Facts::ReleaseInfo),
                typeof(Facts::ReleaseWithdrawal),
                typeof(Facts::RulesetRule),
                typeof(Facts::SalesModelCategoryRestriction),
                typeof(Facts::Theme),
                typeof(Facts::ThemeCategory),
                typeof(Facts::ThemeOrganizationUnit),
                typeof(Facts::UnlimitedOrder),
            };

        public static readonly Type[] AmsFactTypes =
            {
                typeof(Facts::Advertisement),
                typeof(Facts::EntityName),
            };

        public static readonly Type[] AggregateTypes =
            {
                typeof(PriceAggregates::Firm),
                typeof(PriceAggregates::Firm.FirmPosition),
                typeof(PriceAggregates::Firm.FirmAssociatedPosition),
                typeof(PriceAggregates::Firm.FirmDeniedPosition),
                typeof(PriceAggregates::Order),
                typeof(PriceAggregates::Order.OrderPeriod),
                typeof(PriceAggregates::Order.OrderPricePosition),
                typeof(PriceAggregates::Order.OrderCategoryPosition),
                typeof(PriceAggregates::Order.OrderThemePosition),
                typeof(PriceAggregates::Order.AmountControlledPosition),
                typeof(PriceAggregates::Order.ActualPrice),
                typeof(PriceAggregates::Period),
                typeof(PriceAggregates::Price),
                typeof(PriceAggregates::Price.PricePeriod),
                typeof(PriceAggregates::Price.AdvertisementAmountRestriction),
                typeof(PriceAggregates::Price.AssociatedPositionGroupOvercount),

                typeof(AccountAggregates::Order),
                typeof(AccountAggregates::Order.DebtPermission),
                typeof(AccountAggregates::Account),
                typeof(AccountAggregates::Account.AccountPeriod),

                typeof(AdvertisementAggregates::Order),
                typeof(AdvertisementAggregates::Order.MissingAdvertisementReference),
                typeof(AdvertisementAggregates::Order.MissingOrderPositionAdvertisement),
                typeof(AdvertisementAggregates::Order.AdvertisementFailedReview),
                typeof(AdvertisementAggregates::Order.AdvertisementNotBelongToFirm),

                typeof(ConsistencyAggregates::Order),
                typeof(ConsistencyAggregates::Order.BargainSignedLaterThanOrder),
                typeof(ConsistencyAggregates::Order.InvalidFirmAddress),
                typeof(ConsistencyAggregates::Order.InvalidCategory),
                typeof(ConsistencyAggregates::Order.CategoryNotBelongsToAddress),
                typeof(ConsistencyAggregates::Order.HasNoAnyLegalPersonProfile),
                typeof(ConsistencyAggregates::Order.HasNoAnyPosition),
                typeof(ConsistencyAggregates::Order.InactiveReference),
                typeof(ConsistencyAggregates::Order.InvalidBeginDistributionDate),
                typeof(ConsistencyAggregates::Order.InvalidBillsTotal),
                typeof(ConsistencyAggregates::Order.InvalidEndDistributionDate),
                typeof(ConsistencyAggregates::Order.LegalPersonProfileBargainExpired),
                typeof(ConsistencyAggregates::Order.LegalPersonProfileWarrantyExpired),
                typeof(ConsistencyAggregates::Order.MissingBargainScan),
                typeof(ConsistencyAggregates::Order.MissingBills),
                typeof(ConsistencyAggregates::Order.MissingRequiredField),
                typeof(ConsistencyAggregates::Order.MissingOrderScan),

			    typeof(FirmAggregates::Firm),
			    typeof(FirmAggregates::Firm.CategoryPurchase),
			    typeof(FirmAggregates::Order),
			    typeof(FirmAggregates::Order.FirmOrganiationUnitMismatch),
			    typeof(FirmAggregates::Order.InvalidFirm),
                typeof(FirmAggregates::Order.PremiumPartnerProfilePosition),

                typeof(ProjectAggregates::Order),
			    typeof(ProjectAggregates::Order.AddressAdvertisementNonOnTheMap),
			    typeof(ProjectAggregates::Order.CategoryAdvertisement),
			    typeof(ProjectAggregates::Order.CostPerClickAdvertisement),
			    typeof(ProjectAggregates::Project),
			    typeof(ProjectAggregates::Project.Category),
			    typeof(ProjectAggregates::Project.CostPerClickRestriction),
			    typeof(ProjectAggregates::Project.SalesModelRestriction),
			    typeof(ProjectAggregates::Project.NextRelease),

                typeof(ThemeAggregates::Theme),
                typeof(ThemeAggregates::Theme.InvalidCategory),
                typeof(ThemeAggregates::Order),
                typeof(ThemeAggregates::Order.OrderTheme),
                typeof(ThemeAggregates::Project),
                typeof(ThemeAggregates::Project.ProjectDefaultTheme),
            };

        public static readonly Type[] MessagesTypes =
            {
                typeof(Messages::Version),
                typeof(Messages::Version.ValidationResult),
                typeof(Messages::Version.ErmState),
                typeof(Messages::Version.AmsState),
            };

        public static readonly Type[] WebAppTypes =
            {
                typeof(WebApp::Lock),
            };

        public IReadOnlyCollection<Type> Get(ICommand command)
        {
            if (ReferenceEquals(command, BulkReplicationCommands.ErmToFacts) ||
                ReferenceEquals(command, BulkReplicationCommands.ErmToFactsTest))
            {
                return FactTypes;
            }

            if (ReferenceEquals(command, BulkReplicationCommands.FactsToAggregates) ||
                ReferenceEquals(command, BulkReplicationCommands.FactsToAggregatesTest))
            {
                return AggregateTypes;
            }

            if (ReferenceEquals(command, BulkReplicationCommands.AggregatesToMessages) ||
                ReferenceEquals(command, BulkReplicationCommands.AggregatesToMessagesTest))
            {
                return MessagesTypes;
            }

            if (ReferenceEquals(command, BulkReplicationCommands.AmsToFacts))
            {
                return AmsFactTypes;
            }

            throw new ArgumentException($"Data object types cannot be found for command {command}");
        }
    }
}
