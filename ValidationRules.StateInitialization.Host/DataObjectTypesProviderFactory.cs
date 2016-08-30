using System;
using System.Collections.Generic;

using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.DataObjects;
using NuClear.StateInitialization.Core.Factories;
using NuClear.ValidationRules.Storage.Identitites.Connections;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class DataObjectTypesProviderFactory : IDataObjectTypesProviderFactory
    {
        public IDataObjectTypesProvider Create(ReplicateInBulkCommand command)
        {
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is FactsConnectionStringIdentity)
            {
                return new CommandRegardlessDataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(PriceFacts::AssociatedPosition),
                            typeof(PriceFacts::AssociatedPositionsGroup),
                            typeof(PriceFacts::Category),
                            typeof(PriceFacts::DeniedPosition),
                            typeof(PriceFacts::Order),
                            typeof(PriceFacts::OrderPosition),
                            typeof(PriceFacts::OrderPositionAdvertisement),
                            typeof(PriceFacts::OrganizationUnit),
                            typeof(PriceFacts::Position),
                            typeof(PriceFacts::Price),
                            typeof(PriceFacts::PricePosition),
                            typeof(PriceFacts::PricePositionNotActive),
                            typeof(PriceFacts::Project),
                            typeof(PriceFacts::RulesetRule),

                            typeof(AccountFacts::Account),
                            typeof(AccountFacts::Order),
                            typeof(AccountFacts::Project),
                            typeof(AccountFacts::Lock),
                            typeof(AccountFacts::Limit),
                            typeof(AccountFacts::OrderPosition),
                            typeof(AccountFacts::ReleaseWithdrawal),
                        });
            }
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is AggregatesConnectionStringIdentity)
            {
                return new CommandRegardlessDataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(PriceAggregates::AdvertisementAmountRestriction),
                            typeof(PriceAggregates::Order),
                            typeof(PriceAggregates::OrderPeriod),
                            typeof(PriceAggregates::OrderPosition),
                            typeof(PriceAggregates::OrderAssociatedPosition),
                            typeof(PriceAggregates::OrderDeniedPosition),
                            typeof(PriceAggregates::OrderPricePosition),
                            typeof(PriceAggregates::AmountControlledPosition),
                            typeof(PriceAggregates::Period),
                            typeof(PriceAggregates::Position),
                            typeof(PriceAggregates::Price),
                            typeof(PriceAggregates::Project),
                            typeof(PriceAggregates::AssociatedPositionGroupOvercount),
                            typeof(PriceAggregates::PricePeriod),

                            typeof(AccountAggregates::Order),
                            typeof(AccountAggregates::Lock),
                            typeof(AccountAggregates::Account),
                            typeof(AccountAggregates::AccountPeriod),
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}