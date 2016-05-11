using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class NoEntityTypeExplicitMapping : IEntityTypeExplicitMapping
    {
        public IEntityType MapEntityType(IEntityType entityType)
        {
            return entityType;
        }
    }
}