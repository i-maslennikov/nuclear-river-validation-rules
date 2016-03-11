using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public interface IEntityTypeExplicitMapping
    {
        IEntityType MapEntityType(IEntityType entityType);
    }
}