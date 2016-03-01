using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public interface IFindSpecificationProvider<T>
    {
        FindSpecification<T> Create(IEnumerable<AggregateOperation> commands);
    }
}