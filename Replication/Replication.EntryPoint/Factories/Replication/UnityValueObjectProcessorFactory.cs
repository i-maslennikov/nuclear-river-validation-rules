using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityValueObjectProcessorFactory<TValueObject, TEntity, TEntityKey> : IValueObjectProcessorFactory<TEntity>
        where TEntity : IIdentifiable<TEntityKey>
        where TValueObject : class, IObject
    {
        private readonly IUnityContainer _unityContainer;

        public UnityValueObjectProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IValueObjectProcessor<TEntity> Create(IValueObjectMetadata metadata)
        {
            var md = (ValueObjectMetadata<TValueObject, TEntityKey>)metadata;

            return new ValueObjectProcessor<TEntity, TValueObject>(
                ResolveDataChangesDetector(md),
                _unityContainer.Resolve<IBulkRepository<TValueObject>>(),
                ResolveFindSpecificationProvider(md));
        }

        private DataChangesDetector<TValueObject> ResolveDataChangesDetector(ValueObjectMetadata<TValueObject, TEntityKey> metadata)
        {
            var equalityComparerFactory = _unityContainer.Resolve<IEqualityComparerFactory>();
            var query = _unityContainer.Resolve<IQuery>();

            // todo: подумать о типах CompleteMetadataDataChangesDetector + IdentityMetadataDataChangesDetector, и об единственной DependencyOverride
            var detector = new DataChangesDetector<TValueObject>(
                metadata.MapSpecificationProviderForSource,
                metadata.MapSpecificationProviderForTarget,
                equalityComparerFactory.CreateCompleteComparer<TValueObject>(),
                query);

            return detector;
        }

        private IFindSpecificationProvider<TValueObject, TEntity> ResolveFindSpecificationProvider(ValueObjectMetadata<TValueObject, TEntityKey> metadata)
        {
            var identityProvider = _unityContainer.Resolve<IIdentityProvider<TEntityKey>>();
            return new ValueObjectFindSpecificationProvider<TValueObject, TEntity, TEntityKey>(metadata, identityProvider);
        }
    }
}