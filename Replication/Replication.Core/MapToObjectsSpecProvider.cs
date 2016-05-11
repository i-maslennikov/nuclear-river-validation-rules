using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public delegate IEnumerable<TOutput> MapToObjectsSpecProvider<TFilter, out TOutput>(FindSpecification<TFilter> specification);
}