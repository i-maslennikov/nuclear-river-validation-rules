using System.Collections.Generic;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public interface IDataChangesHandler<in TDataObject>
    {
        IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<TDataObject> dataObjects);

        IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<TDataObject> dataObjects);

        IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<TDataObject> dataObjects);

        IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<TDataObject> dataObjects);
    }
}