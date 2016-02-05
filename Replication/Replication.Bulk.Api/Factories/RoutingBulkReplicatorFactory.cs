using System;
using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.Replication.Bulk.API.Storage;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class RoutingBulkReplicatorFactory : IBulkReplicatorFactory
    {
        private readonly DataConnection _sourceDataConnection;
        private readonly DataConnection _targetDataConnection;

        private static readonly IReadOnlyDictionary<Type, Type> RoutingDictionary =
            new Dictionary<Type, Type>
            {
                { typeof(FactMetadata<>), typeof(FactBulkReplicatorFactory<>) },
                { typeof(AggregateMetadata<>), typeof(AggregatesBulkReplicatorFactory<>) },
                { typeof(ValueObjectMetadataElement<>), typeof(ValueObjectsBulkReplicatorFactory<>) },
                { typeof(StatisticsRecalculationMetadata<>), typeof(StatisticsBulkReplicatorFactory<>) }
            };

        public RoutingBulkReplicatorFactory(DataConnection sourceDataConnection, DataConnection targetDataConnection)
        {
            _sourceDataConnection = sourceDataConnection;
            _targetDataConnection = targetDataConnection;
        }

        IReadOnlyCollection<IBulkReplicator> IBulkReplicatorFactory.Create(IMetadataElement metadataElement)
        {
            var metadataElementType = metadataElement.GetType();

            Type factoryType;
            if (!RoutingDictionary.TryGetValue(metadataElementType.GetGenericTypeDefinition(), out factoryType))
            {
                throw new NotSupportedException($"Bulk replication is not supported for the mode described with {metadataElement}");
            }

            var objType = metadataElementType.GenericTypeArguments[0];
            var factory = (IBulkReplicatorFactory)Activator.CreateInstance(factoryType.MakeGenericType(objType), new LinqToDbQuery(_sourceDataConnection), _targetDataConnection);
            return factory.Create(metadataElement);
        }

        public void Dispose()
        {
            _sourceDataConnection.Dispose();
            _targetDataConnection.Dispose();
        }
    }
}