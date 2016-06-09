using System;
using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.ValidationRules.Replication
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            if (typeof(ISyncDataObjectCommand).IsAssignableFrom(typeof(TCommand)))
            {
                return new List<Type>
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
                        typeof(RulesetRule),
                    };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}