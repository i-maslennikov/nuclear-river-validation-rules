using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IChildEntityProcessor<TParentEntityKey>
    {
        void Initialize(IReadOnlyCollection<TParentEntityKey> parentEntityKeys);
        void Recalculate(IReadOnlyCollection<TParentEntityKey> parentEntityKeys);
        void Destroy(IReadOnlyCollection<TParentEntityKey> parentEntityKeys);
    }

    public interface IChildEntityProcessor<TParentEntityKey, TChildEntityKey>
    {
        void RecalculatePartially(IReadOnlyCollection<TParentEntityKey> parentEntityKeys, IReadOnlyCollection<TChildEntityKey> childEntityKeys);
    }
}