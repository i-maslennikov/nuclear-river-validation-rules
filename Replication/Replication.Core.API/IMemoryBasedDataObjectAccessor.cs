using System.Collections.Generic;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API
{
    public interface IMemoryBasedDataObjectAccessor<TDataObject>
    {
        IReadOnlyCollection<TDataObject> GetDataObjects(ICommand command);
        FindSpecification<TDataObject> GetFindSpecification(ICommand command);
    }
}