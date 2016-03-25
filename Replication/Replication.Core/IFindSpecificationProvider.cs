using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public interface IFindSpecificationProvider<TEntity, in TConstraint>
    {
        FindSpecification<TEntity> Create(IEnumerable<TConstraint> constraints);
    }
}