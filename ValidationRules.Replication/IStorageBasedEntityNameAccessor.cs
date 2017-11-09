using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication
{
    public interface IStorageBasedEntityNameAccessor<TEntity> : IStorageBasedDataObjectAccessor<EntityName>
    {
    }
}