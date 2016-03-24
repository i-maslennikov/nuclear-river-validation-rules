using System;

using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class RecalculateAggregateCommandFactory : ICommandFactory<long>
    {
        public IOperation Create(Type entityType, long key)
        {
            return new RecalculateAggregate(entityType, key);
        }
    }
}