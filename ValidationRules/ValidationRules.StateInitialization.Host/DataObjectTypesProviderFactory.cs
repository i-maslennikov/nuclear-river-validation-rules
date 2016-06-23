using System;
using System.Collections.Generic;

using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;
using NuClear.ValidationRules.Storage.Identitites.Connections;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class DataObjectTypesProviderFactory : IDataObjectTypesProviderFactory
    {
        public IDataObjectTypesProvider Create(ReplaceDataObjectsInBulkCommand command)
        {
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is FactsConnectionStringIdentity)
            {
                return new DataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(AssociatedPosition),
                            typeof(AssociatedPositionsGroup),
                            typeof(Category),
                            typeof(DeniedPosition),
                            typeof(Order),
                            typeof(OrderPosition),
                            typeof(OrderPositionAdvertisement),
                            typeof(OrganizationUnit),
                            typeof(Position),
                            typeof(Price),
                            typeof(PricePosition),
                            typeof(PricePositionNotActive),
                            typeof(Project),
                            typeof(RulesetRule)
                        });
            }
            if (command.TargetStorageDescriptor.ConnectionStringIdentity is AggregatesConnectionStringIdentity)
            {
                return new DataObjectTypesProvider(
                    new List<Type>
                        {
                            typeof(Storage.Model.Aggregates.AdvertisementAmountRestriction),
                            typeof(Storage.Model.Aggregates.Order),
                            typeof(Storage.Model.Aggregates.OrderPeriod),
                            typeof(Storage.Model.Aggregates.OrderPosition),
                            typeof(Storage.Model.Aggregates.OrderDeniedPosition),
                            typeof(Storage.Model.Aggregates.OrderPricePosition),
                            typeof(Storage.Model.Aggregates.AmountControlledPosition),
                            typeof(Storage.Model.Aggregates.Period),
                            typeof(Storage.Model.Aggregates.Position),
                            typeof(Storage.Model.Aggregates.Price),
                            typeof(Storage.Model.Aggregates.PriceAssociatedPosition),
                            typeof(Storage.Model.Aggregates.AssociatedPositionGroupOvercount),
                            typeof(Storage.Model.Aggregates.PricePeriod),
                            typeof(Storage.Model.Aggregates.Ruleset),
                            typeof(Storage.Model.Aggregates.RulesetRule)
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}