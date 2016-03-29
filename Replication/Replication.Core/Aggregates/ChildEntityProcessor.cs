using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ChildEntityProcessor<TRootEntity, TChildEntity> : IChildEntityProcessor<TRootEntity>
    {
        private readonly IEntityProcessor<TChildEntity> _childEntity;
        private readonly IFindSpecificationProvider<TChildEntity, long> _findSpecificationProvider;
        private readonly IMapSpecification<FindSpecification<TRootEntity>, FindSpecification<TChildEntity>> _mapSpecification;

        public Type ChildEntityType
            => typeof(TChildEntity);

        public ChildEntityProcessor(IEntityProcessor<TChildEntity> childEntity,
                                    IFindSpecificationProvider<TChildEntity, long> findSpecificationProvider,
                                    IMapSpecification<FindSpecification<TRootEntity>, FindSpecification<TChildEntity>> mapSpecification)
        {
            _childEntity = childEntity;
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public void Initialize(FindSpecification<TRootEntity> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Initialize(spec);
        }

        public void Recalculate(FindSpecification<TRootEntity> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Recalculate(spec);
        }

        public void Destroy(FindSpecification<TRootEntity> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Destroy(spec);
        }

        public void RecalculatePartially(FindSpecification<TRootEntity> specification, IReadOnlyCollection<RecalculateAggregatePart> commands)
        {
            var specFromRoot = _mapSpecification.Map(specification);
            var specFromCommands = _findSpecificationProvider.Create(commands.Select(x => x.EntityInstanceId));
            _childEntity.Recalculate(specFromRoot & specFromCommands);
        }
    }
}