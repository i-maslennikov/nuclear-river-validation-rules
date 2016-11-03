using System;
using System.Collections.Generic;

using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.DataObjects;
using NuClear.StateInitialization.Core.Factories;
using NuClear.ValidationRules.Storage.Identitites.Connections;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using AdvertisementFacts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using FirmFacts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using ProjectFacts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using ThemeFacts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class DataObjectTypesProviderFactory : IDataObjectTypesProviderFactory
    {
        public IDataObjectTypesProvider Create(ReplicateInBulkCommand command)
        {
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is FactsConnectionStringIdentity)
            {
                return new CommandRegardlessDataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(PriceFacts::AssociatedPosition),
                            typeof(PriceFacts::AssociatedPositionsGroup),
                            typeof(PriceFacts::Category),
                            typeof(PriceFacts::DeniedPosition),
                            typeof(PriceFacts::Order),
                            typeof(PriceFacts::OrderPosition),
                            typeof(PriceFacts::OrderPositionAdvertisement),
                            typeof(PriceFacts::Position),
                            typeof(PriceFacts::Price),
                            typeof(PriceFacts::PricePosition),
                            typeof(PriceFacts::PricePositionNotActive),
                            typeof(PriceFacts::Project),
                            typeof(PriceFacts::RulesetRule),
                            typeof(PriceFacts::Theme),

                            typeof(AccountFacts::Account),
                            typeof(AccountFacts::Order),
                            typeof(AccountFacts::Project),
                            typeof(AccountFacts::Lock),
                            typeof(AccountFacts::Limit),
                            typeof(AccountFacts::OrderPosition),
                            typeof(AccountFacts::ReleaseWithdrawal),

                            typeof(AdvertisementFacts::AdvertisementElementTemplate),
                            typeof(AdvertisementFacts::AdvertisementElement),
                            typeof(AdvertisementFacts::AdvertisementTemplate),
                            typeof(AdvertisementFacts::Advertisement),
                            typeof(AdvertisementFacts::Firm),
                            typeof(AdvertisementFacts::FirmAddress),
                            typeof(AdvertisementFacts::FirmAddressWebsite),
                            typeof(AdvertisementFacts::Position),
                            typeof(AdvertisementFacts::PricePosition),
                            typeof(AdvertisementFacts::OrderPositionAdvertisement),
                            typeof(AdvertisementFacts::OrderPosition),
                            typeof(AdvertisementFacts::Order),
                            typeof(AdvertisementFacts::Project),

                            typeof(ConsistencyFacts::Bargain),
                            typeof(ConsistencyFacts::BargainScanFile),
                            typeof(ConsistencyFacts::Bill),
                            typeof(ConsistencyFacts::BranchOffice),
                            typeof(ConsistencyFacts::BranchOfficeOrganizationUnit),
                            typeof(ConsistencyFacts::Category),
                            typeof(ConsistencyFacts::CategoryFirmAddress),
                            typeof(ConsistencyFacts::Deal),
                            typeof(ConsistencyFacts::Firm),
                            typeof(ConsistencyFacts::FirmAddress),
                            typeof(ConsistencyFacts::LegalPerson),
                            typeof(ConsistencyFacts::LegalPersonProfile),
                            typeof(ConsistencyFacts::Order),
                            typeof(ConsistencyFacts::OrderPosition),
                            typeof(ConsistencyFacts::OrderPositionAdvertisement),
                            typeof(ConsistencyFacts::OrderScanFile),
                            typeof(ConsistencyFacts::Position),
                            typeof(ConsistencyFacts::Project),
                            typeof(ConsistencyFacts::ReleaseWithdrawal),

                            typeof(FirmFacts::Firm),
                            typeof(FirmFacts::FirmAddress),
                            typeof(FirmFacts::FirmAddressCategory),
                            typeof(FirmFacts::Order),
                            typeof(FirmFacts::OrderPosition),
                            typeof(FirmFacts::OrderPositionAdvertisement),
                            typeof(FirmFacts::SpecialPosition),
                            typeof(FirmFacts::Project),

                            typeof(ProjectFacts::Category),
                            typeof(ProjectFacts::CategoryOrganizationUnit),
                            typeof(ProjectFacts::CostPerClickCategoryRestriction),
                            typeof(ProjectFacts::FirmAddress),
                            typeof(ProjectFacts::Order),
                            typeof(ProjectFacts::OrderPosition),
                            typeof(ProjectFacts::OrderPositionAdvertisement),
                            typeof(ProjectFacts::OrderPositionCostPerClick),
                            typeof(ProjectFacts::Position),
                            typeof(ProjectFacts::PricePosition),
                            typeof(ProjectFacts::Project),
                            typeof(ProjectFacts::ReleaseInfo),
                            typeof(ProjectFacts::SalesModelCategoryRestriction),

                            typeof(ThemeFacts::Theme),
                            typeof(ThemeFacts::ThemeCategory),
                            typeof(ThemeFacts::ThemeOrganizationUnit),
                            typeof(ThemeFacts::Category),
                            typeof(ThemeFacts::Order),
                            typeof(ThemeFacts::OrderPosition),
                            typeof(ThemeFacts::OrderPositionAdvertisement),
                            typeof(ThemeFacts::Project),
                        });
            }

            if (command.TargetStorageDescriptor.ConnectionStringIdentity is AggregatesConnectionStringIdentity)
            {
                return new CommandRegardlessDataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(PriceAggregates::AdvertisementAmountRestriction),
                            typeof(PriceAggregates::Order),
                            typeof(PriceAggregates::OrderPeriod),
                            typeof(PriceAggregates::OrderPosition),
                            typeof(PriceAggregates::OrderAssociatedPosition),
                            typeof(PriceAggregates::OrderDeniedPosition),
                            typeof(PriceAggregates::OrderPricePosition),
                            typeof(PriceAggregates::AmountControlledPosition),
                            typeof(PriceAggregates::Period),
                            typeof(PriceAggregates::Position),
                            typeof(PriceAggregates::Price),
                            typeof(PriceAggregates::Project),
                            typeof(PriceAggregates::AssociatedPositionGroupOvercount),
                            typeof(PriceAggregates::PricePeriod),
                            typeof(PriceAggregates::Theme),
                            typeof(PriceAggregates::Category),

                            typeof(AccountAggregates::Order),
                            typeof(AccountAggregates::Lock),
                            typeof(AccountAggregates::Account),
                            typeof(AccountAggregates::AccountPeriod),

                            typeof(AdvertisementAggregates::Order),
                            typeof(AdvertisementAggregates::Order.MissingAdvertisementReference),
                            typeof(AdvertisementAggregates::Order.MissingOrderPositionAdvertisement),
                            typeof(AdvertisementAggregates::Order.AdvertisementDeleted),
                            typeof(AdvertisementAggregates::Order.AdvertisementMustBelongToFirm),
                            typeof(AdvertisementAggregates::Order.AdvertisementIsDummy),
                            typeof(AdvertisementAggregates::Order.CouponDistributionPeriod),
                            typeof(AdvertisementAggregates::Order.OrderPositionAdvertisement),
                            typeof(AdvertisementAggregates::Advertisement),
                            typeof(AdvertisementAggregates::Advertisement.AdvertisementWebsite),
                            typeof(AdvertisementAggregates::Advertisement.RequiredElementMissing),
                            typeof(AdvertisementAggregates::Advertisement.ElementNotPassedReview),
                            typeof(AdvertisementAggregates::Advertisement.ElementOffsetInDays),
                            typeof(AdvertisementAggregates::AdvertisementElementTemplate),
                            typeof(AdvertisementAggregates::Firm),
                            typeof(AdvertisementAggregates::Firm.FirmWebsite),
                            typeof(AdvertisementAggregates::Firm.WhiteListDistributionPeriod),
                            typeof(AdvertisementAggregates::Position),

                            typeof(ConsistencyAggregates::Order),
                            typeof(ConsistencyAggregates::Order.BargainSignedLaterThanOrder),
                            typeof(ConsistencyAggregates::Order.InvalidFirm),
                            typeof(ConsistencyAggregates::Order.InvalidFirmAddress),
                            typeof(ConsistencyAggregates::Order.InvalidCategory),
                            typeof(ConsistencyAggregates::Order.InvalidCategoryFirmAddress),
                            typeof(ConsistencyAggregates::Order.HasNoAnyLegalPersonProfile),
                            typeof(ConsistencyAggregates::Order.HasNoAnyPosition),
                            typeof(ConsistencyAggregates::Order.InactiveReference),
                            typeof(ConsistencyAggregates::Order.InvalidBeginDistributionDate),
                            typeof(ConsistencyAggregates::Order.InvalidBillsPeriod),
                            typeof(ConsistencyAggregates::Order.InvalidBillsTotal),
                            typeof(ConsistencyAggregates::Order.InvalidEndDistributionDate),
                            typeof(ConsistencyAggregates::Order.LegalPersonProfileBargainExpired),
                            typeof(ConsistencyAggregates::Order.LegalPersonProfileWarrantyExpired),
                            typeof(ConsistencyAggregates::Order.MissingBargainScan),
                            typeof(ConsistencyAggregates::Order.MissingBills),
                            typeof(ConsistencyAggregates::Order.MissingRequiredField),
                            typeof(ConsistencyAggregates::Order.MissingOrderScan),

                            typeof(FirmAggregates::Firm),
                            typeof(FirmAggregates::Firm.AdvantageousPurchasePositionDistributionPeriod),
                            typeof(FirmAggregates::Order),
                            typeof(FirmAggregates::Order.CategoryPurchase),
                            typeof(FirmAggregates::Order.FirmOrganiationUnitMismatch),
                            typeof(FirmAggregates::Order.NotApplicapleForDesktopPosition),
                            typeof(FirmAggregates::Order.SelfAdvertisementPosition),

                            typeof(ProjectAggregates::Category),
                            typeof(ProjectAggregates::FirmAddress),
                            typeof(ProjectAggregates::Order),
                            typeof(ProjectAggregates::Order.AddressAdvertisement),
                            typeof(ProjectAggregates::Order.CategoryAdvertisement),
                            typeof(ProjectAggregates::Order.CostPerClickAdvertisement),
                            typeof(ProjectAggregates::Position),
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
                            typeof(ThemeAggregates::Category),
                        });
            }

            if (command.TargetStorageDescriptor.ConnectionStringIdentity is MessagesConnectionStringIdentity)
            {
                return new CommandRegardlessDataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(Messages::Version.ValidationResult),
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}
