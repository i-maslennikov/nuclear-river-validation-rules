using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.Api.Replicators;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.Api.Factories
{
    public class AggregatesBulkReplicatorFactory<T> : IBulkReplicatorFactory
        where T : class, IIdentifiable<long>
    {
        private readonly IQuery _query;
        private readonly DataConnection _dataConnection;

        public AggregatesBulkReplicatorFactory(IQuery query, DataConnection dataConnection)
        {
            _query = query;
            _dataConnection = dataConnection;
        }

        public IReadOnlyCollection<IBulkReplicator> Create(IMetadataElement metadataElement)
        {
            var aggregateMetadata = (AggregateMetadata<T>)metadataElement;

            return new IBulkReplicator[] { new InsertsBulkReplicator<T>(_query, _dataConnection, aggregateMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) }
                .Concat(aggregateMetadata.Elements
                                         .OfType<IValueObjectMetadataElement>()
                                         .SelectMany(valueObjectMetadata =>
                                                     {
                                                         var factoryType = typeof(ValueObjectsBulkReplicatorFactory<>).MakeGenericType(valueObjectMetadata.ValueObjectType);
                                                         var factory = (IBulkReplicatorFactory)Activator.CreateInstance(factoryType, _query, _dataConnection);
                                                         return factory.Create(valueObjectMetadata);
                                                     }))
                .ToArray();
        }

        public void Dispose()
        {
        }
    }
}