using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;


namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceAggregatesSchema = "PriceAggregates";
        private const string ProjectAggregatesSchema = "ProjectAggregates";
        private const string AccountAggregatesSchema = "AccountAggregates";
        private const string AdvertisementAggregatesSchema = "AdvertisementAggregates";
        private const string ConsistencyAggregatesSchema = "ConsistencyAggregates";
        private const string FirmAggregatesSchema = "FirmAggregates";
        private const string ThemeAggregatesSchema = "ThemeAggregates";

        public static MappingSchema Aggregates
            => new MappingSchema(nameof(Aggregates), new SqlServerMappingSchema())
                .RegisterDataTypes()
                .GetFluentMappingBuilder()
                .RegisterPriceAggregates()
                .RegisterProjectAggregates()
                .RegisterAccountAggregates()
                .RegisterAdvertisementAggregates()
                .RegisterConsistencyAggregates()
                .RegisterFirmAggregates()
                .RegisterThemeAggregates()
                .MappingSchema;

        private static FluentMappingBuilder RegisterThemeAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<ThemeAggregates::Theme>()
                   .HasSchemaName(ThemeAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeAggregates::Theme.InvalidCategory>()
                   .HasSchemaName(ThemeAggregatesSchema);

            builder.Entity<ThemeAggregates::Order>()
                   .HasSchemaName(ThemeAggregatesSchema);
            builder.Entity<ThemeAggregates::Order.OrderTheme>()
                   .HasSchemaName(ThemeAggregatesSchema);

            builder.Entity<ThemeAggregates::Project>()
                   .HasSchemaName(ThemeAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeAggregates::Project.ProjectDefaultTheme>()
                   .HasSchemaName(ThemeAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterFirmAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<FirmAggregates::Firm>()
                   .HasSchemaName(FirmAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmAggregates::Firm.AdvantageousPurchasePositionDistributionPeriod>()
                   .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order>()
                   .HasSchemaName(FirmAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmAggregates::Order.CategoryPurchase>()
                   .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.FirmOrganiationUnitMismatch>()
                   .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.InvalidFirm>()
                  .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.NotApplicapleForDesktopPosition>()
                   .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.SelfAdvertisementPosition>()
                   .HasSchemaName(FirmAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterPriceAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<PriceAggregates::Price>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Price.AssociatedPositionGroupOvercount>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Price.AdvertisementAmountRestriction>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Firm>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Firm.FirmPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Firm.FirmAssociatedPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Firm.FirmDeniedPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Period.OrderPeriod>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.OrderPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.OrderPricePosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.AmountControlledPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.ActualPrice>()
                   .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Period>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Start)
                  .HasPrimaryKey(x => x.End)
                  .HasPrimaryKey(x => x.ProjectId);

            builder.Entity<PriceAggregates::Period.PricePeriod>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Position>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterProjectAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<ProjectAggregates::FirmAddress>()
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ProjectAggregates::Order>()
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ProjectAggregates::Order.AddressAdvertisement>()
                   .HasSchemaName(ProjectAggregatesSchema);

            builder.Entity<ProjectAggregates::Order.CategoryAdvertisement>()
                   .HasSchemaName(ProjectAggregatesSchema);

            builder.Entity<ProjectAggregates::Order.CostPerClickAdvertisement>()
                   .HasSchemaName(ProjectAggregatesSchema);

            builder.Entity<ProjectAggregates::Project>()
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ProjectAggregates::Project.Category>()
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasTableName("ProjectCategory");

            builder.Entity<ProjectAggregates::Project.CostPerClickRestriction>()
                   .HasSchemaName(ProjectAggregatesSchema);

            builder.Entity<ProjectAggregates::Project.SalesModelRestriction>()
                   .HasSchemaName(ProjectAggregatesSchema);

            builder.Entity<ProjectAggregates::Project.NextRelease>()
                   .HasSchemaName(ProjectAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterAccountAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<AccountAggregates::Order>()
                  .HasSchemaName(AccountAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountAggregates::Order.DebtPermission>()
                  .HasSchemaName(AccountAggregatesSchema);

            builder.Entity<AccountAggregates::Order.Lock>()
                   .HasSchemaName(AccountAggregatesSchema);

            builder.Entity<AccountAggregates::Account>()
                   .HasSchemaName(AccountAggregatesSchema);

            builder.Entity<AccountAggregates::Account.AccountPeriod>()
                   .HasSchemaName(AccountAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterAdvertisementAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<AdvertisementAggregates::Order>()
                  .HasSchemaName(AdvertisementAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementAggregates::Order.MissingAdvertisementReference>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.MissingOrderPositionAdvertisement>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.AdvertisementDeleted>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.AdvertisementMustBelongToFirm>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.AdvertisementIsDummy>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.CouponDistributionPeriod>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.OrderPositionAdvertisement>()
                   .HasSchemaName(AdvertisementAggregatesSchema);

            builder.Entity<AdvertisementAggregates::Advertisement>()
                  .HasSchemaName(AdvertisementAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementAggregates::Advertisement.AdvertisementWebsite>()
                  .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Advertisement.RequiredElementMissing>()
                  .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Advertisement.ElementNotPassedReview>()
                  .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Advertisement.Coupon>()
                  .HasSchemaName(AdvertisementAggregatesSchema);

            builder.Entity<AdvertisementAggregates::Firm>()
                  .HasSchemaName(AdvertisementAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementAggregates::Firm.FirmWebsite>()
                  .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Firm.WhiteListDistributionPeriod>()
                  .HasSchemaName(AdvertisementAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterConsistencyAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<ConsistencyAggregates::Order>()
                  .HasSchemaName(ConsistencyAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyAggregates::Order.BargainSignedLaterThanOrder>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InvalidFirmAddress>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.CategoryNotBelongsToAddress>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InvalidCategory>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.HasNoAnyLegalPersonProfile>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.HasNoAnyPosition>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InactiveReference>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InvalidBeginDistributionDate>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InvalidBillsPeriod>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InvalidBillsTotal>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.InvalidEndDistributionDate>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.LegalPersonProfileBargainExpired>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.LegalPersonProfileWarrantyExpired>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.MissingBargainScan>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.MissingBills>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.MissingRequiredField>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            builder.Entity<ConsistencyAggregates::Order.MissingOrderScan>()
                  .HasSchemaName(ConsistencyAggregatesSchema);

            return builder;
        }
    }
}
