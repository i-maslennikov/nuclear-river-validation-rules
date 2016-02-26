using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class ValueObjectsBulkReplicatorFactory<TValueObject, TKey> : IBulkReplicatorFactory 
        where TValueObject : class
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
            var statisticsRecalculationMetadata = (ValueObjectMetadata<TValueObject, TKey>)metadataElement;
            return new[] { new InsertsBulkReplicator<TValueObject>(_query, _dataConnection, statisticsRecalculationMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<TValueObject>())) };
        }

        public void Dispose()
        {
        }
    }
}