using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class NullDataChangesHandler<TDataObject> : IDataChangesHandler<TDataObject>
    {
        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<TDataObject> dataObjects) => Array.Empty<IEvent>();
    }
}