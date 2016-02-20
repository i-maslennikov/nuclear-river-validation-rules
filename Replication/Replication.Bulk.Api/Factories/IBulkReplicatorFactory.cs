using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;

namespace NuClear.Replication.Bulk.API.Factories
{
    public interface IBulkReplicatorFactory : IDisposable
    {
        IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement);
    }
}