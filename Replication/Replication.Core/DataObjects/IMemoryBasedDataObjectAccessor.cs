using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.DataObjects
{
    public interface IMemoryBasedDataObjectAccessor<TDataObject>
    {
        IReadOnlyCollection<TDataObject> GetDataObjects(ICommand command);
        FindSpecification<TDataObject> GetFindSpecification(ICommand command);
    }
}