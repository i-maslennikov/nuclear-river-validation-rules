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
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PrincipalPositionId)
                      .HasPrimaryKey(x => x.DeniedPositionId)
                      .HasPrimaryKey(x => x.ObjectBindingType);

                config.Entity<PriceAssociatedPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PrincipalPositionId)
                      .HasPrimaryKey(x => x.AssociatedPositionId)
                      .HasPrimaryKey(x => x.GroupId)
                      .HasPrimaryKey(x => x.ObjectBindingType);

                config.Entity<AdvertisementAmountRestriction>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PositionId)
                      .HasPrimaryKey(x => x.Min)
                      .HasPrimaryKey(x => x.Max);

                config.Entity<Ruleset>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<RulesetRule>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.RulesetId)
                      .HasPrimaryKey(x => x.RuleType)
                      .HasPrimaryKey(x => x.PrincipalPositionId)
                      .HasPrimaryKey(x => x.ObjectBindingType)
                      .HasPrimaryKey(x => x.DependentPositionId);

                config.Entity<Order>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<OrderPeriod>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Start)
                      .HasPrimaryKey(x => x.OrganizationUnitId)
                      .HasPrimaryKey(x => x.OrderId);

                config.Entity<OrderPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.OrderId)
                      .HasPrimaryKey(x => x.ItemPositionId)
                      .HasPrimaryKey(x => x.CompareMode)
                      .HasPrimaryKey(x => x.Category3Id)
                      .HasPrimaryKey(x => x.FirmAddressId);

                config.Entity<OrderPrice>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.OrderId)
                      .HasPrimaryKey(x => x.PriceId);

                config.Entity<Period>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Start)
                      .HasPrimaryKey(x => x.End)
                      .HasPrimaryKey(x => x.OrganizationUnitId);

                config.Entity<Position>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<PricePeriod>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.OrganizationUnitId)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.Start);

                return schema;
            }
        }
    }
}
