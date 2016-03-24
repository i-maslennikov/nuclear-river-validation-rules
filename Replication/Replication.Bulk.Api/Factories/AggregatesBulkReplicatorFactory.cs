using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class AggregatesBulkReplicatorFactory<T, TKey> : IBulkReplicatorFactory
        where T : class, IIdentifiable<TKey>
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
            var aggregateMetadata = (AggregateMetadata<T, TKey>)metadataElement;

            return new IBulkReplicator[] { new InsertsBulkReplicator<T>(_query, _dataConnection, aggregateMetadata.MapSpecificationProviderForSource.Invoke(Specs.Find.All<T>())) }
                .Concat(aggregateMetadata.Elements
                                         .OfType<IValueObjectMetadata>()
                                         .SelectMany(metadata =>
                                                     {
                                                         var factoryType = typeof(ValueObjectsBulkReplicatorFactory<,>).MakeGenericType(metadata.ValueObjectType, metadata.EntityKeyType);
                                                         var factory = (IBulkReplicatorFactory)Activator.CreateInstance(factoryType, _query, _dataConnection);
                                                         return factory.Create(metadata);
                                                     }))
                .ToArray();
        }

        public void Dispose()
        {
        }
    }
}