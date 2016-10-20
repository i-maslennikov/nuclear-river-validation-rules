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
                config.Entity<Advertisement>().HasSchemaName(BillingSchema).HasTableName("Advertisements").HasPrimaryKey(x => x.Id);
                config.Entity<AdvertisementTemplate>().HasSchemaName(BillingSchema).HasTableName("AdvertisementTemplates").HasPrimaryKey(x => x.Id);
                config.Entity<AdvertisementElement>().HasSchemaName(BillingSchema).HasTableName("AdvertisementElements").HasPrimaryKey(x => x.Id);
                config.Entity<AdvertisementElementStatus>().HasSchemaName(BillingSchema).HasTableName("AdvertisementElementStatuses").HasPrimaryKey(x => x.Id);
                config.Entity<AdvertisementElementTemplate>().HasSchemaName(BillingSchema).HasTableName("AdvertisementElementTemplates").HasPrimaryKey(x => x.Id);
                config.Entity<AssociatedPositionsGroup>().HasSchemaName(BillingSchema).HasTableName("AssociatedPositionsGroups").HasPrimaryKey(x => x.Id);
                config.Entity<AssociatedPosition>().HasSchemaName(BillingSchema).HasTableName("AssociatedPositions").HasPrimaryKey(x => x.Id);
                config.Entity<BranchOffice>().HasSchemaName(BillingSchema).HasTableName("BranchOffices").HasPrimaryKey(x => x.Id);
                config.Entity<BranchOfficeOrganizationUnit>().HasSchemaName(BillingSchema).HasTableName("BranchOfficeOrganizationUnits").HasPrimaryKey(x => x.Id);
                config.Entity<Deal>().HasSchemaName(BillingSchema).HasTableName("Deals").HasPrimaryKey(x => x.Id);
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
                config.Entity<OrderPositionCostPerClick>().HasSchemaName(BillingSchema).HasTableName("OrderPositionCostPerClicks");
                config.Entity<OrderPositionAdvertisement>().HasSchemaName(BillingSchema).HasTableName("OrderPositionAdvertisement").HasPrimaryKey(x => x.Id);
                config.Entity<OrganizationUnit>().HasSchemaName(BillingSchema).HasTableName("OrganizationUnits").HasPrimaryKey(x => x.Id);
                config.Entity<Price>().HasSchemaName(BillingSchema).HasTableName("Prices").HasPrimaryKey(x => x.Id);
                config.Entity<PricePosition>().HasSchemaName(BillingSchema).HasTableName("PricePositions").HasPrimaryKey(x => x.Id);
                config.Entity<Project>().HasSchemaName(BillingSchema).HasTableName("Projects").HasPrimaryKey(x => x.Id);
                config.Entity<Position>().HasSchemaName(BillingSchema).HasTableName("Positions").HasPrimaryKey(x => x.Id);
                config.Entity<PositionChild>().HasSchemaName(BillingSchema).HasTableName("PositionChildren")
                    .HasPrimaryKey(x => x.MasterPositionId)
                    .HasPrimaryKey(x => x.ChildPositionId);
                config.Entity<Category>().HasSchemaName(BusinessDirectorySchema).HasTableName("Categories").HasPrimaryKey(x => x.Id);
                config.Entity<CategoryOrganizationUnit>().HasSchemaName(BusinessDirectorySchema).HasTableName("CategoryOrganizationUnits").HasPrimaryKey(x => x.Id);
                config.Entity<CategoryFirmAddress>().HasSchemaName(BusinessDirectorySchema).HasTableName("CategoryFirmAddresses").HasPrimaryKey(x => x.Id);
                config.Entity<CostPerClickCategoryRestriction>().HasSchemaName(BusinessDirectorySchema).HasTableName("CostPerClickCategoryRestrictions");
                config.Entity<SalesModelCategoryRestriction>().HasSchemaName(BusinessDirectorySchema).HasTableName("SalesModelCategoryRestrictions");
                config.Entity<User>().HasSchemaName(SecuritySchema).HasTableName("Users").HasPrimaryKey(x => x.Id);
                config.Entity<UserProfile>().HasSchemaName(SecuritySchema).HasTableName("UserProfiles").HasPrimaryKey(x => x.Id);
                config.Entity<TimeZone>().HasSchemaName(SharedSchema).HasTableName("TimeZones").HasPrimaryKey(x => x.Id);
                config.Entity<Theme>().HasSchemaName(BillingSchema).HasTableName("Themes").HasPrimaryKey(x => x.Id);
                config.Entity<ThemeCategory>().HasSchemaName(BillingSchema).HasTableName("ThemeCategories").HasPrimaryKey(x => x.Id);
                config.Entity<ThemeOrganizationUnit>().HasSchemaName(BillingSchema).HasTableName("ThemeOrganizationUnits").HasPrimaryKey(x => x.Id);

                config.Entity<Bargain>().HasSchemaName(BillingSchema).HasTableName("Bargains").HasPrimaryKey(x => x.Id);
                config.Entity<BargainFile>().HasSchemaName(BillingSchema).HasTableName("BargainFiles").HasPrimaryKey(x => x.Id);
                config.Entity<Bill>().HasSchemaName(BillingSchema).HasTableName("Bills").HasPrimaryKey(x => x.Id);
                config.Entity<Firm>().HasSchemaName(BusinessDirectorySchema).HasTableName("Firms").HasPrimaryKey(x => x.Id);
                config.Entity<FirmAddress>().HasSchemaName(BusinessDirectorySchema).HasTableName("FirmAddresses").HasPrimaryKey(x => x.Id);
                config.Entity<LegalPerson>().HasSchemaName(BillingSchema).HasTableName("LegalPersons").HasPrimaryKey(x => x.Id);
                config.Entity<LegalPersonProfile>().HasSchemaName(BillingSchema).HasTableName("LegalPersonProfiles").HasPrimaryKey(x => x.Id);
                config.Entity<OrderFile>().HasSchemaName(BillingSchema).HasTableName("OrderFiles").HasPrimaryKey(x => x.Id);

                return schema;
            }
        }
    }
}
