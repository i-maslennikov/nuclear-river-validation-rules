using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public interface IChildEntityProcessor<TRootEntityKey>
    {
        Type ChildEntityType { get; }
        void Initialize(IReadOnlyCollection<TRootEntityKey> specification);
        void Recalculate(IReadOnlyCollection<TRootEntityKey> specification);
        void Destroy(IReadOnlyCollection<TRootEntityKey> specification);
        void RecalculatePartially(IReadOnlyCollection<TRootEntityKey> specification, IReadOnlyCollection<RecalculateAggregatePart> commands);
    }
}