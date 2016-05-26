using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.Aggregates;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceAggregateSchema = "PriceAggregate";

        public static MappingSchema Aggregates
        {
            get
            {
                var schema = new MappingSchema(new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<Price>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<PriceDeniedPosition>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<PriceAssociatedPosition>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<AdvertisementAmountRestriction>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<Ruleset>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<RulesetRule>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<Order>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<OrderPeriod>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<OrderPosition>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<OrderPrice>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<Period>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Start)
                      .HasPrimaryKey(x => x.End)
                      .HasPrimaryKey(x => x.OrganizationUnitId);

                config.Entity<PricePeriod>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<Position>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                return schema;
            }
        }
    }
}
