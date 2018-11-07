using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using SystemAggregates = NuClear.ValidationRules.Storage.Model.SystemRules.Aggregates;

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
        private const string SystemAggregatesSchema = "SystemAggregates";

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
                .RegisterSystemAggregates()
                .MappingSchema;

        private static FluentMappingBuilder RegisterSystemAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<SystemAggregates::SystemStatus>()
                   .HasSchemaName(SystemAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterThemeAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<ThemeAggregates::Theme>()
                   .HasSchemaName(ThemeAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeAggregates::Theme.InvalidCategory>()
                   .HasSchemaName(ThemeAggregatesSchema);

            builder.Entity<ThemeAggregates::Order>()
                   .HasSchemaName(ThemeAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);
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

            builder.Entity<FirmAggregates::Firm.CategoryPurchase>()
                   .HasSchemaName(FirmAggregatesSchema)
                   .HasIndex(x => new { x.FirmId, x.Begin, x.End, x.CategoryId }, x => new { x.Scope });

            builder.Entity<FirmAggregates::Order>()
                   .HasSchemaName(FirmAggregatesSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.FirmId, x.Begin, x.End }, x => new { x.Id, x.Scope });

            builder.Entity<FirmAggregates::Order.FirmOrganiationUnitMismatch>()
                   .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.InvalidFirm>()
                  .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.PartnerPosition>()
                   .HasSchemaName(FirmAggregatesSchema);

            builder.Entity<FirmAggregates::Order.FmcgCutoutPosition>()
                   .HasSchemaName(FirmAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterPriceAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<PriceAggregates::Firm>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Firm.FirmPosition>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.FirmId, x.ItemPositionId, x.Begin },
                             x => new { x.OrderId, x.OrderPositionId, x.PackagePositionId, x.HasNoBinding, x.Category1Id, x.Category3Id, x.FirmAddressId, x.Scope, x.End })
                   .HasIndex(x => new { x.OrderId }, x => new { x.FirmId, x.OrderPositionId, x.PackagePositionId, x.ItemPositionId, x.Scope, x.Begin, x.End });

            builder.Entity<PriceAggregates::Firm.FirmAssociatedPosition>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.FirmId, x.OrderPositionId, x.ItemPositionId }, x => new { x.PrincipalPositionId, x.BindingType });

            builder.Entity<PriceAggregates::Firm.FirmDeniedPosition>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.FirmId, x.OrderPositionId, x.ItemPositionId }, x => new { x.DeniedPositionId, x.BindingType, x.Begin, x.End });

            builder.Entity<PriceAggregates::Order>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Order.OrderPeriod>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.OrderId }, x => new { x.Begin, x.End, x.Scope });

            builder.Entity<PriceAggregates::Order.OrderPricePosition>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.OrderId })
                   .HasIndex(x => new { x.PriceId })
                   .HasIndex(x => new { x.IsActive });

            builder.Entity<PriceAggregates::Order.OrderCategoryPosition>()
                   .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.OrderThemePosition>()
                   .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.AmountControlledPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order.ActualPrice>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.OrderId, x.PriceId });

            builder.Entity<PriceAggregates::Order.EntranceControlledPosition>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasIndex(x => new { x.OrderId }, x => new { x.EntranceCode, x.FirmAddressId})
                   .HasIndex(x => new { x.EntranceCode });

            builder.Entity<PriceAggregates::Period>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Start);

            builder.Entity<PriceAggregates::Ruleset>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Ruleset.AdvertisementAmountRestriction>()
                   .HasSchemaName(PriceAggregatesSchema)
                   .HasPrimaryKey(x => new { x.RulesetId, x.ProjectId, x.CategoryCode });

            return builder;
        }

        private static FluentMappingBuilder RegisterProjectAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<ProjectAggregates::Order>()
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ProjectAggregates::Order.AddressAdvertisementNonOnTheMap>()
                   .HasSchemaName(ProjectAggregatesSchema);

            builder.Entity<ProjectAggregates::Order.CategoryAdvertisement>()
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasIndex(x => new { x.OrderId, x.IsSalesModelRestrictionApplicable }, x => new { x.OrderPositionId, x.PositionId, x.CategoryId, x.SalesModel });

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
                   .HasSchemaName(ProjectAggregatesSchema)
                   .HasIndex(x => new { x.ProjectId, x.CategoryId, x.Begin, x.End }, x => new { x.SalesModel });

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
            builder.Entity<AdvertisementAggregates::Order.AdvertisementFailedReview>()
                   .HasSchemaName(AdvertisementAggregatesSchema);
            builder.Entity<AdvertisementAggregates::Order.AdvertisementNotBelongToFirm>()
                   .HasSchemaName(AdvertisementAggregatesSchema);

            return builder;
        }

        private static FluentMappingBuilder RegisterConsistencyAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<ConsistencyAggregates::Order>()
                   .HasSchemaName(ConsistencyAggregatesSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.Id }, x => new { x.BeginDistribution, x.EndDistributionPlan });

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

            builder.Entity<ConsistencyAggregates::Order.MissingValidPartnerFirmAddresses>()
                   .HasSchemaName(ConsistencyAggregatesSchema)
                   .HasIndex(x => new { x.OrderId });

            return builder;
        }
    }
}
