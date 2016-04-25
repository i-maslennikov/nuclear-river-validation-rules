using System;
using System.Collections.Generic;

namespace NuClear.Replication.Core.API.DataObjects
{
    public sealed class NullDataChangesHandler<TDataObject> : IDataChangesHandler<TDataObject>
    {
        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
    }
}