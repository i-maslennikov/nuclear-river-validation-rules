using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IEntityProcessor<TEntity>
    {
        void Initialize(FindSpecification<TEntity> specification);
        void Recalculate(FindSpecification<TEntity> specification);
        void Destroy(FindSpecification<TEntity> specification);
    }
}