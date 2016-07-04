using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceAggregateSchema = "PriceAggregate";
        private const string AccountAggregateSchema = "AccountAggregate";

        public static MappingSchema Aggregates
            => new MappingSchema(nameof(Aggregates), new SqlServerMappingSchema())
                .GetFluentMappingBuilder()
                .RegisterPriceAggregates()
                .RegisterAccountAggregates()
                .MappingSchema;

        private static FluentMappingBuilder RegisterPriceAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<PriceAggregates::Price>()
                  .HasSchemaName(PriceAggregateSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::AssociatedPositionGroupOvercount>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::AdvertisementAmountRestriction>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::Order>()
                  .HasSchemaName(PriceAggregateSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::OrderPeriod>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::OrderPosition>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::OrderAssociatedPosition>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::OrderDeniedPosition>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::OrderPricePosition>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::AmountControlledPosition>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::Period>()
                  .HasSchemaName(PriceAggregateSchema)
                  .HasPrimaryKey(x => x.Start)
                  .HasPrimaryKey(x => x.End)
                  .HasPrimaryKey(x => x.ProjectId);

            builder.Entity<PriceAggregates::PricePeriod>()
                  .HasSchemaName(PriceAggregateSchema);

            builder.Entity<PriceAggregates::Position>()
                  .HasSchemaName(PriceAggregateSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterAccountAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<AccountAggregates::Order>()
                  .HasSchemaName(AccountAggregateSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }
    }
}
