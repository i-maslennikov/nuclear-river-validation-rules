using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string FactsSchema = "Facts";

        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema(nameof(Facts), new SqlServerMappingSchema());
                schema.GetFluentMappingBuilder()
                      .RegisterFacts();

                return schema;
            }
        }

        private static FluentMappingBuilder RegisterFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<Account>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<Advertisement>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementElement>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementElementTemplate>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementTemplate>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AssociatedPosition>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AssociatedPositionsGroup>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<Bargain>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<BargainScanFile>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Bill>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<BranchOffice>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<BranchOfficeOrganizationUnit>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Category>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<CategoryOrganizationUnit>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<CostPerClickCategoryRestriction>()
                   .HasSchemaName(FactsSchema);
            builder.Entity<Deal>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<DeniedPosition>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<Firm>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<FirmAddress>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<FirmAddressCategory>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<FirmAddressWebsite>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<LegalPerson>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<LegalPersonProfile>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Lock>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<Order>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<OrderPosition>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<OrderPositionAdvertisement>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<OrderPositionCostPerClick>()
                   .HasSchemaName(FactsSchema);
            builder.Entity<OrderScanFile>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Position>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PositionChild>()
                  .HasSchemaName(FactsSchema);
            builder.Entity<Price>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PricePosition>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<Project>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<ReleaseInfo>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ReleaseWithdrawal>()
              .HasSchemaName(FactsSchema)
              .HasPrimaryKey(x => x.Id);
            // TODO: хак чтобы не делать факты со сложным ключом
            builder.Entity<RulesetRule>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id)
                  .HasPrimaryKey(x => x.RuleType)
                  .HasPrimaryKey(x => x.DependentPositionId)
                  .HasPrimaryKey(x => x.PrincipalPositionId);
            builder.Entity<SalesModelCategoryRestriction>()
                   .HasSchemaName(FactsSchema);
            builder.Entity<Theme>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeCategory>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeOrganizationUnit>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<UnlimitedOrder>()
                  .HasSchemaName(FactsSchema)
                  .HasPrimaryKey(x => x.OrderId)
                  .HasPrimaryKey(x => x.PeriodStart);
            return builder;
        }
    }
}
