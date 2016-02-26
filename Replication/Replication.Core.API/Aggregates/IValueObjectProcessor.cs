using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessor
    {
        void ApplyChanges(IReadOnlyCollection<AggregateOperation> commands);
    }
}
