using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessor<TEntity>
    {
        void Execute(IReadOnlyCollection<TEntity> commands);
    }
}
