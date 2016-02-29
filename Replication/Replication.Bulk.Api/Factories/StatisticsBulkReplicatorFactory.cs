using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.API.Factories
{
    // todo: уже идентичен ValueObjectsBulkReplicatorFactory, можно подумать об удалении
    public class StatisticsBulkReplicatorFactory<T, TKey> : IBulkReplicatorFactory where T : class
    {
        private readonly IQuery _query;
        private readonly DataConnection _dataConnection;

        public StatisticsBulkReplicatorFactory(IQuery query, DataConnection dataConnection)
        {
            _query = query;
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadata)
        {
            var statisticsRecalculationMetadata = (StatisticsRecalculationMetadata<T, TKey>)metadata;
            return new[] { new InsertsBulkReplicator<T>(_query, _dataConnection, statisticsRecalculationMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) };
        }

        public void Dispose()
        {
        }
    }
}