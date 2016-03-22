using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class IdentityEntityTypeMapping : IEntityTypeExplicitMapping
    {
        public IEntityType MapEntityType(IEntityType entityType)
        {
            return entityType;
        }
    }
}