using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public interface IFindSpecificationProvider<TEntity, in TConstaint>
    {
        FindSpecification<TEntity> Create(IEnumerable<TConstaint> constraints);
    }
}