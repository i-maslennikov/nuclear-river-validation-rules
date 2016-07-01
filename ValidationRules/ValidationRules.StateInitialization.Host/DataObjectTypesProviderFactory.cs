using System;
using System.Collections.Generic;

using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;
using NuClear.ValidationRules.Storage.Identitites.Connections;
using NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

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
                            typeof(Storage.Model.PriceRules.Aggregates.AdvertisementAmountRestriction),
                            typeof(Storage.Model.PriceRules.Aggregates.Order),
                            typeof(Storage.Model.PriceRules.Aggregates.OrderPeriod),
                            typeof(Storage.Model.PriceRules.Aggregates.OrderPosition),
                            typeof(Storage.Model.PriceRules.Aggregates.OrderAssociatedPosition),
                            typeof(Storage.Model.PriceRules.Aggregates.OrderDeniedPosition),
                            typeof(Storage.Model.PriceRules.Aggregates.OrderPricePosition),
                            typeof(Storage.Model.PriceRules.Aggregates.AmountControlledPosition),
                            typeof(Storage.Model.PriceRules.Aggregates.Period),
                            typeof(Storage.Model.PriceRules.Aggregates.Position),
                            typeof(Storage.Model.PriceRules.Aggregates.Price),
                            typeof(Storage.Model.PriceRules.Aggregates.AssociatedPositionGroupOvercount),
                            typeof(Storage.Model.PriceRules.Aggregates.PricePeriod),
                        });
            }

            throw new ArgumentException($"Instance of type IDataObjectTypesProvider cannot be created for connection string name {command.TargetStorageDescriptor.MappingSchema}");
        }
    }
}