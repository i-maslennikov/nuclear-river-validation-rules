using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IStatisticsRecalculator
    {
        void Recalculate(IReadOnlyCollection<IOperation> commands);
    }
}