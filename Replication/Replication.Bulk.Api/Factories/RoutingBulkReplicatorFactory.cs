using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Bulk.Api.Storage;
using NuClear.Replication.Bulk.API.Replicators;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class RoutingBulkReplicatorFactory : IBulkReplicatorFactory
    {
        private readonly IStorage _source;
        private readonly IStorage _target;

        private static readonly IReadOnlyDictionary<Type, Type> RoutingDictionary =
            new Dictionary<Type, Type>
            {
                { typeof(FactMetadata<>), typeof(FactBulkReplicatorFactory<>) },
                { typeof(AggregateMetadata<,>), typeof(AggregatesBulkReplicatorFactory<,>) },
                { typeof(ValueObjectMetadata<,>), typeof(ValueObjectsBulkReplicatorFactory<,>) }
            };

        public RoutingBulkReplicatorFactory(IStorage source, IStorage target)
        {
            _source = source;
            _target = target;
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
            var factory = (IBulkReplicatorFactory)Activator.CreateInstance(concreteFactoryType, _source.GetReadAccess(), _target.GetWriteAccess());
            return factory.Create(metadataElement);
        }

        public void Dispose()
        {
            _source.Dispose();
            _target.Dispose();
        }
    }
}