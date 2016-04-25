using System.Collections.Generic;

namespace NuClear.Replication.Core.DataObjects
{
    public interface IDataChangesHandler<in TDataObject>
    {
        IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<TDataObject> dataObjects);
        IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<TDataObject> dataObjects);
        IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<TDataObject> dataObjects);
        IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<TDataObject> dataObjects);
    }
}