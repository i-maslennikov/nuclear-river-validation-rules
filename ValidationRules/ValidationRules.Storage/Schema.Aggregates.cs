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

                config.Entity<DeniedPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PositionId)
                      .HasPrimaryKey(x => x.DeniedPositionId);

                config.Entity<MasterPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PositionId)
                      .HasPrimaryKey(x => x.MasterPositionId);

                config.Entity<Order>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<OrderPeriod>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.OrderId)
                      .HasPrimaryKey(x => x.PeriodId);

                config.Entity<OrderPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.OrderId)
                      .HasPrimaryKey(x => x.PositionId);

                config.Entity<OrderPrice>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.OrderId)
                      .HasPrimaryKey(x => x.PriceId);

                config.Entity<Period>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<Position>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<Price>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<PricePeriod>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PeriodId);

                config.Entity<PricePosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PositionId);

                return schema;
            }
        }
    }
}
