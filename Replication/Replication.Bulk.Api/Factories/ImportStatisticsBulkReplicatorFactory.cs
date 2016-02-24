using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Factories;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Bulk.Api.Factories
{
    public class ImportStatisticsBulkReplicatorFactory<T1, T2> : IBulkReplicatorFactory
        where T1 : class
        where T2 : class
    {
        private readonly IQuery _source;
        private readonly DataConnection _target;

        public ImportStatisticsBulkReplicatorFactory(IQuery source, DataConnection target)
        {
            _source = source;
            _target = target;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement)
        {
            var importMetadataElement = (ImportStatisticsMetadata<T1, T2>)metadataElement;
            var replicator = new InsertsBulkReplicator<T1>(
                _source, _target, new MapSpecification<IQuery, IEnumerable<T1>>(query => importMetadataElement.MapSpecification.Map(query.For<T2>().Single())));
            return new[] { replicator };
        }

        public void Dispose()
        {
        }
    }
}