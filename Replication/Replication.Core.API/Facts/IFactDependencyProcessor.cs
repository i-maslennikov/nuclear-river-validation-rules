using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactDependencyProcessor
    {
        DependencyType DependencyType { get; }
        IEnumerable<IOperation> ProcessCreation(IReadOnlyCollection<long> factIds);
        IEnumerable<IOperation> ProcessUpdating(IReadOnlyCollection<long> factIds);
        IEnumerable<IOperation> ProcessDeletion(IReadOnlyCollection<long> factIds);
    }
}
