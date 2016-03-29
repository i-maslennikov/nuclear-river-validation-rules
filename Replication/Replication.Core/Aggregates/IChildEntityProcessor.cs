using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public interface IChildEntityProcessor<TRootEntity>
    {
        Type ChildEntityType { get; }
        void Initialize(IReadOnlyCollection<TRootEntity> specification);
        void Recalculate(IReadOnlyCollection<TRootEntity> specification);
        void Destroy(IReadOnlyCollection<TRootEntity> specification);
        void RecalculatePartially(IReadOnlyCollection<TRootEntity> specification, IReadOnlyCollection<RecalculateAggregatePart> commands);
    }
}