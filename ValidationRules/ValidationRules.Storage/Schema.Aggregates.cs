﻿using LinqToDB.DataProvider.SqlServer;
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
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.DeniedPositionId)
                      .HasPrimaryKey(x => x.PrincipalPositionId);

                config.Entity<PriceAssociatedPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.AssociatedPositionId)
                      .HasPrimaryKey(x => x.PrincipalPositionId);

                config.Entity<AdvertisementAmountRestriction>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PositionId);

                config.Entity<Ruleset>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<RulesetDeniedPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.RulesetId)
                      .HasPrimaryKey(x => x.DeniedPositionId)
                      .HasPrimaryKey(x => x.PrincipalPositionId);

                config.Entity<RulesetAssociatedPosition>()
                      .HasSchemaName(PriceAggregateSchema)
                      .HasPrimaryKey(x => x.RulesetId)
                      .HasPrimaryKey(x => x.AssociatedPositionId)
                      .HasPrimaryKey(x => x.PrincipalPositionId);

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
                      .HasPrimaryKey(x => x.ItemPositionId)
                      .HasPrimaryKey(x => x.PackagePositionId);

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
                      .HasPrimaryKey(x => x.PriceId)
                      .HasPrimaryKey(x => x.PeriodId);

                return schema;
            }
        }
    }
}
