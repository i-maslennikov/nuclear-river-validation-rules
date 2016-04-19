using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.ValidationRules.Domain.Model;

namespace NuClear.ValidationRules.Domain.Commands
{
    public class RecalculatePeriodCommandFactory : ICommandFactory<PeriodKey>
    {
        public IOperation Create(IEntityType entityType, PeriodKey key)
        {
            return new RecalculateAggregate(new EntityReference(entityType, key));
        }
    }
}