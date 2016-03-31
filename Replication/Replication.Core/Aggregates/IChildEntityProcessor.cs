using System.Collections.Generic;

namespace NuClear.Replication.Core.Aggregates
{
    public interface IChildEntityProcessor<TRootEntityKey>
    {
        void Initialize(IReadOnlyCollection<TRootEntityKey> specification);
        void Recalculate(IReadOnlyCollection<TRootEntityKey> specification);
        void Destroy(IReadOnlyCollection<TRootEntityKey> specification);
    }

    public interface IChildEntityProcessor<TRootEntityKey, TChildEntityKey>
    {
        void RecalculatePartially(IReadOnlyCollection<TRootEntityKey> specification, IReadOnlyCollection<TChildEntityKey> commands);
    }
}