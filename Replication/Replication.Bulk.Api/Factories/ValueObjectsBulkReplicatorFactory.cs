using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class ValueObjectsBulkReplicatorFactory<T> : IBulkReplicatorFactory where T : class
    {
        private readonly IQuery _query;
        private readonly DataConnection _dataConnection;

        public ValueObjectsBulkReplicatorFactory(IQuery query, DataConnection dataConnection)
        {
            _query = query;
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement)
        {
            var statisticsRecalculationMetadata = (ValueObjectMetadataElement<T>)metadataElement;
            return new[] { new InsertsBulkReplicator<T>(_query, _dataConnection, statisticsRecalculationMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) };
        }

        public void Dispose()
        {
        }
    }
}