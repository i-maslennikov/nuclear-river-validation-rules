using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public interface IChildEntityProcessorFactory<TRootEntityKey>
    {
        IChildEntityProcessor<TRootEntityKey> Create(IAggregateMetadata parent, IAggregateMetadata child, IChildEntityFeature childFeature);
    }

    public class UnityChildEntityProcessorFactory<TRootEntityKey, TChildEntity> : IChildEntityProcessorFactory<TRootEntityKey>
        where TChildEntity : IIdentifiable<long>
    {
        private readonly IUnityContainer _unityContainer;

        public UnityChildEntityProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IChildEntityProcessor<TRootEntityKey> Create(IAggregateMetadata parent, IAggregateMetadata child, IChildEntityFeature childFeature)
        {
            var entityProcessorFactory = _unityContainer.Resolve<UnityEntityProcessorFactory<TChildEntity>>();
            var entityProcessor = entityProcessorFactory.Create(child);
            var feature = parent.Features.OfType<ChildEntityFeature<TRootEntityKey, TChildEntity, long>>().Single();

            return new ChildEntityProcessor<TRootEntityKey, TChildEntity>(
                entityProcessor,
                _unityContainer.Resolve<FindSpecificationProvider<TChildEntity, long>>(),
                new MapSpecification<IReadOnlyCollection<TRootEntityKey>, FindSpecification<TChildEntity>>(feature.FindSpecificationProvider));
        }
    }
}