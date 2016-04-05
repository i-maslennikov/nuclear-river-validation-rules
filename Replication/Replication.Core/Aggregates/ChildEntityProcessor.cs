using System.Collections.Generic;

using NuClear.Replication.Core.API.Aggregates;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ChildEntityProcessor<TParentEntityKey, TChildEntity, TChildEntityKey> : IChildEntityProcessor<TParentEntityKey>, IChildEntityProcessor<TParentEntityKey, TChildEntityKey>
    {
        private readonly IEntityProcessor<TChildEntity> _childEntity;
        private readonly IFindSpecificationProvider<TChildEntity, TChildEntityKey> _findSpecificationProvider;
        private readonly IMapSpecification<IReadOnlyCollection<TParentEntityKey>, FindSpecification<TChildEntity>> _mapSpecification;

        public ChildEntityProcessor(IEntityProcessor<TChildEntity> childEntity,
                                    IFindSpecificationProvider<TChildEntity, TChildEntityKey> findSpecificationProvider,
                                    IMapSpecification<IReadOnlyCollection<TParentEntityKey>, FindSpecification<TChildEntity>> mapSpecification)
        {
            _childEntity = childEntity;
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public void Initialize(IReadOnlyCollection<TParentEntityKey> parentEntityKeys)
        {
            var spec = _mapSpecification.Map(parentEntityKeys);
            _childEntity.Initialize(spec);
        }

        public void Recalculate(IReadOnlyCollection<TParentEntityKey> parentEntityKeys)
        {
            var spec = _mapSpecification.Map(parentEntityKeys);
            _childEntity.Recalculate(spec);
        }

        public void Destroy(IReadOnlyCollection<TParentEntityKey> parentEntityKeys)
        {
            var spec = _mapSpecification.Map(parentEntityKeys);
            _childEntity.Destroy(spec);
        }

        public void RecalculatePartially(IReadOnlyCollection<TParentEntityKey> parentEntityKeys, IReadOnlyCollection<TChildEntityKey> childEntityKeys)
        {
            var specFromRoot = _mapSpecification.Map(parentEntityKeys);
            var specFromCommands = _findSpecificationProvider.Create(childEntityKeys);
            _childEntity.Recalculate(specFromRoot & specFromCommands);
        }
    }
}