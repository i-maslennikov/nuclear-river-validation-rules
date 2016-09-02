using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string BillingSchema = "Billing";
        private const string BusinessDirectorySchema = "BusinessDirectory";
        private const string OrderValidationSchema = "OrderValidation";
        private const string SecuritySchema = "Security";
        private const string SharedSchema = "Shared";

        public static MappingSchema Erm
        {
            get
            {
                var schema = new MappingSchema(nameof(Erm), new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<Account>().HasSchemaName(BillingSchema).HasTableName("Accounts").HasPrimaryKey(x => x.Id);
                config.Entity<AssociatedPositionsGroup>().HasSchemaName(BillingSchema).HasTableName("AssociatedPositionsGroups").HasPrimaryKey(x => x.Id);
                config.Entity<AssociatedPosition>().HasSchemaName(BillingSchema).HasTableName("AssociatedPositions").HasPrimaryKey(x => x.Id);
                config.Entity<DeniedPosition>().HasSchemaName(BillingSchema).HasTableName("DeniedPositions").HasPrimaryKey(x => x.Id);
                config.Entity<ReleaseInfo>().HasSchemaName(BillingSchema).HasTableName("ReleaseInfos").HasPrimaryKey(x => x.Id);
                config.Entity<ReleaseWithdrawal>().HasSchemaName(BillingSchema).HasTableName("ReleasesWithdrawals").HasPrimaryKey(x => x.Id);
                config.Entity<Ruleset>().HasSchemaName(OrderValidationSchema).HasTableName("Rulesets").HasPrimaryKey(x => x.Id);
                config.Entity<RulesetRule>().HasSchemaName(OrderValidationSchema).HasTableName("RulesetRules")
                      .HasPrimaryKey(x => x.RulesetId)
                      .HasPrimaryKey(x => x.RuleType)
                      .HasPrimaryKey(x => x.DependentPositionId)
                      .HasPrimaryKey(x => x.PrincipalPositionId);
                config.Entity<Limit>().HasSchemaName(BillingSchema).HasTableName("Limits").HasPrimaryKey(x => x.Id);
                config.Entity<Lock>().HasSchemaName(BillingSchema).HasTableName("Locks").HasPrimaryKey(x => x.Id);
                config.Entity<Order>().HasSchemaName(BillingSchema).HasTableName("Orders").HasPrimaryKey(x => x.Id);
                config.Entity<OrderPosition>().HasSchemaName(BillingSchema).HasTableName("OrderPositions").HasPrimaryKey(x => x.Id);
                config.Entity<OrderPositionAdvertisement>().HasSchemaName(BillingSchema).HasTableName("OrderPositionAdvertisement").HasPrimaryKey(x => x.Id);
                config.Entity<OrganizationUnit>().HasSchemaName(BillingSchema).HasTableName("OrganizationUnits").HasPrimaryKey(x => x.Id);
                config.Entity<Price>().HasSchemaName(BillingSchema).HasTableName("Prices").HasPrimaryKey(x => x.Id);
                config.Entity<PricePosition>().HasSchemaName(BillingSchema).HasTableName("PricePositions").HasPrimaryKey(x => x.Id);
                config.Entity<Project>().HasSchemaName(BillingSchema).HasTableName("Projects").HasPrimaryKey(x => x.Id);
                config.Entity<Position>().HasSchemaName(BillingSchema).HasTableName("Positions").HasPrimaryKey(x => x.Id);
                config.Entity<Category>().HasSchemaName(BusinessDirectorySchema).HasTableName("Categories").HasPrimaryKey(x => x.Id);
                config.Entity<User>().HasSchemaName(SecuritySchema).HasTableName("Users").HasPrimaryKey(x => x.Id);
                config.Entity<UserProfile>().HasSchemaName(SecuritySchema).HasTableName("UserProfiles").HasPrimaryKey(x => x.Id);
                config.Entity<TimeZone>().HasSchemaName(SharedSchema).HasTableName("TimeZones").HasPrimaryKey(x => x.Id);
                config.Entity<Theme>().HasSchemaName(BillingSchema).HasTableName("Themes").HasPrimaryKey(x => x.Id);

                config.Entity<Bargain>().HasSchemaName(BillingSchema).HasTableName("Bargains").HasPrimaryKey(x => x.Id);
                config.Entity<BargainFile>().HasSchemaName(BillingSchema).HasTableName("BargainFiles").HasPrimaryKey(x => x.Id);
                config.Entity<Bill>().HasSchemaName(BillingSchema).HasTableName("Bills").HasPrimaryKey(x => x.Id);
                config.Entity<Firm>().HasSchemaName(BusinessDirectorySchema).HasTableName("Firms").HasPrimaryKey(x => x.Id);
                config.Entity<FirmAddress>().HasSchemaName(BusinessDirectorySchema).HasTableName("FirmAddresses").HasPrimaryKey(x => x.Id);
                config.Entity<LegalPersonProfile>().HasSchemaName(BillingSchema).HasTableName("LegalPersonProfiles").HasPrimaryKey(x => x.Id);
                config.Entity<OrderFile>().HasSchemaName(BillingSchema).HasTableName("OrderFiles").HasPrimaryKey(x => x.Id);

                return schema;
            }
        }
    }
}
