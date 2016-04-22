using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityEntityProcessorFactory<TEntity>
    {
        private readonly IUnityContainer _unityContainer;

        public UnityEntityProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public EntityProcessor<TEntity> Create(IMetadataElement metadata)
        {
            return new EntityProcessor<TEntity>(
                _unityContainer.Resolve<IBulkRepository<TEntity>>(),
                ResolveDataChangesDetectorDependency(metadata),
                ResolveValueObjectProcessorsDepencency(metadata));
        }

        private IReadOnlyCollection<IValueObjectProcessor<TEntity>> ResolveValueObjectProcessorsDepencency(IMetadataElement metadata)
        {
            return metadata.Elements.OfType<IValueObjectMetadata>().Select(CreateProcessor).ToArray();
        }

        private IValueObjectProcessor<TEntity> CreateProcessor(IValueObjectMetadata metadata)
        {
            var factoryType = typeof(UnityValueObjectProcessorFactory<,,>).MakeGenericType(metadata.ValueObjectType, typeof(TEntity), metadata.EntityKeyType);
            var processorFactory = (IValueObjectProcessorFactory<TEntity>)_unityContainer.Resolve(factoryType);
            var processor = processorFactory.Create(metadata);
            return processor;
        }

        private DataChangesDetector<TEntity> ResolveDataChangesDetectorDependency(IMetadataElement metadata)
        {
            var aggregateMetadata = (IAggregateMetadata<TEntity>)metadata;
            var equalityComparerFactory = _unityContainer.Resolve<IEqualityComparerFactory>();
            var query = _unityContainer.Resolve<IQuery>();

            return new DataChangesDetector<TEntity>(
                aggregateMetadata.MapSpecificationProviderForSource,
                aggregateMetadata.MapSpecificationProviderForTarget,
                equalityComparerFactory.CreateIdentityComparer<TEntity>(),
                query);
        }
    }
}