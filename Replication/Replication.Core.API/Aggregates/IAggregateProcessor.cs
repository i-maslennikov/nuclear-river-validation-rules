using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateProcessor
    {
        void Initialize(IReadOnlyCollection<InitializeAggregate> commands);
        void Recalculate(IReadOnlyCollection<RecalculateAggregate> commands);
        void Recalculate(IReadOnlyCollection<RecalculateAggregatePart> commands);
        void Destroy(IReadOnlyCollection<DestroyAggregate> commands);
    }
}
