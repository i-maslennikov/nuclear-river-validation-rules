using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class NoEntityTypeExplicitMapping : IEntityTypeExplicitMapping
    {
        public IEntityType MapEntityType(IEntityType entityType)
        {
            return entityType;
        }
    }
}