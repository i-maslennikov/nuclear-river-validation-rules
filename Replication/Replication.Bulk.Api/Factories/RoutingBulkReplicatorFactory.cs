using System;
using System.Collections.Generic;

using LinqToDB.Data;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.River.Common.Metadata.Elements;

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
                { typeof(AggregateMetadata<,>), typeof(AggregatesBulkReplicatorFactory<,>) },
                { typeof(ValueObjectMetadata<,>), typeof(ValueObjectsBulkReplicatorFactory<,>) },
                { typeof(StatisticsRecalculationMetadata<,>), typeof(StatisticsBulkReplicatorFactory<,>) }
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

            var concreteFactoryType = factoryType.MakeGenericType(metadataElementType.GenericTypeArguments);
            var factory = (IBulkReplicatorFactory)Activator.CreateInstance(concreteFactoryType, new LinqToDbQuery(_sourceDataConnection), _targetDataConnection);
            return factory.Create(metadataElement);
        }

        public void Dispose()
        {
            _sourceDataConnection.Dispose();
            _targetDataConnection.Dispose();
        }
    }
}