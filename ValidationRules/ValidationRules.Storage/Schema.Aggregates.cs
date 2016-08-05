﻿using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceAggregatesSchema = "PriceAggregates";
        private const string AccountAggregatesSchema = "AccountAggregates";

        public static MappingSchema Aggregates
            => new MappingSchema(nameof(Aggregates), new SqlServerMappingSchema())
                .GetFluentMappingBuilder()
                .RegisterPriceAggregates()
                .RegisterAccountAggregates()
                .MappingSchema;

        private static FluentMappingBuilder RegisterPriceAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<PriceAggregates::Price>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::AssociatedPositionGroupOvercount>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::AdvertisementAmountRestriction>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Order>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::OrderPeriod>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::OrderPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::OrderAssociatedPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::OrderDeniedPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::OrderPricePosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::AmountControlledPosition>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Period>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Start)
                  .HasPrimaryKey(x => x.End)
                  .HasPrimaryKey(x => x.ProjectId);

            builder.Entity<PriceAggregates::PricePeriod>()
                  .HasSchemaName(PriceAggregatesSchema);

            builder.Entity<PriceAggregates::Position>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<PriceAggregates::Project>()
                  .HasSchemaName(PriceAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterAccountAggregates(this FluentMappingBuilder builder)
        {
            builder.Entity<AccountAggregates::Order>()
                  .HasSchemaName(AccountAggregatesSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountAggregates::Lock>()
                   .HasSchemaName(AccountAggregatesSchema);

            builder.Entity<AccountAggregates::Account>()
                   .HasSchemaName(AccountAggregatesSchema);

            builder.Entity<AccountAggregates::AccountPeriod>()
                   .HasSchemaName(AccountAggregatesSchema);

            return builder;
        }
    }
}
