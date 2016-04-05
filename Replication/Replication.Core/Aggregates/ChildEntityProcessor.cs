using System.Collections.Generic;

using NuClear.Replication.Core.API.Aggregates;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ChildEntityProcessor<TRootEntityKey, TChildEntity, TChildEntityKey> : IChildEntityProcessor<TRootEntityKey>, IChildEntityProcessor<TRootEntityKey, TChildEntityKey>
    {
        private readonly IEntityProcessor<TChildEntity> _childEntity;
        private readonly IFindSpecificationProvider<TChildEntity, TChildEntityKey> _findSpecificationProvider;
        private readonly IMapSpecification<IReadOnlyCollection<TRootEntityKey>, FindSpecification<TChildEntity>> _mapSpecification;

        public ChildEntityProcessor(IEntityProcessor<TChildEntity> childEntity,
                                    IFindSpecificationProvider<TChildEntity, TChildEntityKey> findSpecificationProvider,
                                    IMapSpecification<IReadOnlyCollection<TRootEntityKey>, FindSpecification<TChildEntity>> mapSpecification)
        {
            _childEntity = childEntity;
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public void Initialize(IReadOnlyCollection<TRootEntityKey> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Initialize(spec);
        }

        public void Recalculate(IReadOnlyCollection<TRootEntityKey> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Recalculate(spec);
        }

        public void Destroy(IReadOnlyCollection<TRootEntityKey> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Destroy(spec);
        }

        public void RecalculatePartially(IReadOnlyCollection<TRootEntityKey> specification, IReadOnlyCollection<TChildEntityKey> commands)
        {
            var specFromRoot = _mapSpecification.Map(specification);
            var specFromCommands = _findSpecificationProvider.Create(commands);
            _childEntity.Recalculate(specFromRoot & specFromCommands);
        }
    }
}