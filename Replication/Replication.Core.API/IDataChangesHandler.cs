using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API
{
    public interface IDataChangesHandler<in TDataObject>
    {
        IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<TDataObject> dataObjects);
        IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<TDataObject> dataObjects);
        IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<TDataObject> dataObjects);
        IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<TDataObject> dataObjects);
    }
}