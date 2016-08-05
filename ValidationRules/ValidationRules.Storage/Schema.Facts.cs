using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceContextSchema = "PriceFacts";
        private const string AccountContextSchema = "AccountFacts";
        private const string ConsistencyFactsSchema = "ConsistencyFacts";

        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema(nameof(Facts), new SqlServerMappingSchema());
                schema.GetFluentMappingBuilder()
                      .RegisterPriceFacts()
                      .RegisterAccountFacts()
                      .RegisterConsistencyFacts();

                return schema;
            }
        }

        private static FluentMappingBuilder RegisterPriceFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<PriceFacts::AssociatedPositionsGroup>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::AssociatedPosition>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::DeniedPosition>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Order>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::OrderPosition>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::OrderPositionAdvertisement>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::OrganizationUnit>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Price>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::PricePosition>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::PricePositionNotActive>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Project>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Position>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Category>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Theme>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id);

            // TODO: хак чтобы не делать факты со сложным ключом
            builder.Entity<PriceFacts::RulesetRule>()
                  .HasSchemaName(PriceContextSchema)
                  .HasPrimaryKey(x => x.Id)
                  .HasPrimaryKey(x => x.RuleType)
                  .HasPrimaryKey(x => x.DependentPositionId)
                  .HasPrimaryKey(x => x.PrincipalPositionId);

            return builder;
        }

        private static FluentMappingBuilder RegisterAccountFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<AccountFacts::Order>()
                  .HasSchemaName(AccountContextSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Account>()
                  .HasSchemaName(AccountContextSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Project>()
                  .HasSchemaName(AccountContextSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Lock>()
                  .HasSchemaName(AccountContextSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Limit>()
              .HasSchemaName(AccountContextSchema)
              .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::ReleaseWithdrawal>()
              .HasSchemaName(AccountContextSchema)
              .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::OrderPosition>()
              .HasSchemaName(AccountContextSchema)
              .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterConsistencyFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<ConsistencyFacts::Order>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            return builder;
        }
    }
}
