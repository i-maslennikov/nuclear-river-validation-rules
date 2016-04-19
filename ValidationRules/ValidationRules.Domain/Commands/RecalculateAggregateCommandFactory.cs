using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.ValidationRules.Domain.Commands
{
    public class RecalculateAggregateCommandFactory : ICommandFactory<long>
    {
        public IOperation Create(IEntityType entityType, long key)
        {
            return new RecalculateAggregate(new EntityReference(entityType, key));
        }
    }
}