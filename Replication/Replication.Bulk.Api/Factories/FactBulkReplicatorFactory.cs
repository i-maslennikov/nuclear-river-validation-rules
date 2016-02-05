using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class FactBulkReplicatorFactory<T> : IBulkReplicatorFactory where T : class, IIdentifiable
    {
        private readonly IQuery _query;
        private readonly DataConnection _dataConnection;

        public FactBulkReplicatorFactory(IQuery query, DataConnection dataConnection)
        {
            _query = query;
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement) 
        {
            var factMetadata = (FactMetadata<T>)metadataElement;
            return new[] { new InsertsBulkReplicator<T>(_query, _dataConnection, factMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) };
        }

        public void Dispose()
        {
        }
    }
}