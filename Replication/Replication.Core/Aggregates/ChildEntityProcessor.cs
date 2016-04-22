using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Aggregates;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ChildEntityProcessor<TParentEntityKey, TChildEntity, TChildEntityKey> : IChildEntityProcessor<TParentEntityKey>
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

        public Type EntityType
            => typeof(TChildEntity);

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

        public void Recalculate(IDictionary<TParentEntityKey, IReadOnlyCollection<object>> entityKeys)
        {
            FindSpecification<TChildEntity> spec = null;
            foreach (var entityKey in entityKeys)
            {
                var byParent = _mapSpecification.Map(new [] { entityKey.Key });
                var bySelf = _findSpecificationProvider.Create(entityKey.Value.Cast<TChildEntityKey>());

                spec = spec == null
                           ? byParent & bySelf
                           : spec | (byParent & bySelf);
            }

            _childEntity.Recalculate(spec);
        }
    }
}