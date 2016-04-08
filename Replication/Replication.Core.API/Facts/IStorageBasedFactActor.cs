using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IStorageBasedFactActor<TDataObject>
    {
        IEqualityComparer<TDataObject> DataObjectEqualityComparer { get; }

        IQueryable<TDataObject> GetDataObjectsSource(IQuery query);

        FindSpecification<TDataObject> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands);

        IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<TDataObject> dataObjects);

        IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<TDataObject> dataObjects);

        IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<TDataObject> dataObjects);

        IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<TDataObject> dataObjects);
    }
}