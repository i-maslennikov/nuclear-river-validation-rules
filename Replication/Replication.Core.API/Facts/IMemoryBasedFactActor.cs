using System.Collections.Generic;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IMemoryBasedFactActor<TDataObject>
    {
        IReadOnlyCollection<TDataObject> GetDataObjects(ICommand command);
        FindSpecification<TDataObject> GetDataObjectsFindSpecification(ICommand command);
        IReadOnlyCollection<IEvent> HandleChanges(IReadOnlyCollection<TDataObject> dataObjects);
    }
}