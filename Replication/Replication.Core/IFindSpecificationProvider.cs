using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public interface IFindSpecificationProvider<T, in TCommand>
    {
        FindSpecification<T> Create(IEnumerable<TCommand> commands);
    }
}