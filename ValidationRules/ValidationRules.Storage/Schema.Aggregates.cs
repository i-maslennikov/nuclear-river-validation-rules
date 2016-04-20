using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Domain.Model.Aggregates;

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

                config.Entity<RulesetDeniedPosition>()
                      .HasSchemaName(PriceAggregateSchema);

                config.Entity<RulesetAssociatedPosition>()
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

                config.Entity<Position>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<PricePeriod>()
                      .HasSchemaName(PriceAggregateSchema);

                return schema;
            }
        }
    }
}
