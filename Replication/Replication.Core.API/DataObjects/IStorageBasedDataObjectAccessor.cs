using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API.DataObjects
{
    public interface IStorageBasedDataObjectAccessor<TDataObject>
    {
        IQueryable<TDataObject> GetSource();

        FindSpecification<TDataObject> GetFindSpecification(IReadOnlyCollection<ICommand> commands);
    }
}