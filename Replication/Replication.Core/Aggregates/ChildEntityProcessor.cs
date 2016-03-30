using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    // todo: не поддерживает обобщённый ключ вследствие привязки к конкретной команде RecalculateAggregatePart
    public sealed class ChildEntityProcessor<TRootEntityKey, TChildEntity> : IChildEntityProcessor<TRootEntityKey>
    {
        private readonly IEntityProcessor<TChildEntity> _childEntity;
        private readonly IFindSpecificationProvider<TChildEntity, long> _findSpecificationProvider;
        private readonly IMapSpecification<IReadOnlyCollection<TRootEntityKey>, FindSpecification<TChildEntity>> _mapSpecification;

        public Type ChildEntityType
            => typeof(TChildEntity);

        public ChildEntityProcessor(IEntityProcessor<TChildEntity> childEntity,
                                    IFindSpecificationProvider<TChildEntity, long> findSpecificationProvider,
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

        public void RecalculatePartially(IReadOnlyCollection<TRootEntityKey> specification, IReadOnlyCollection<RecalculateAggregatePart> commands)
        {
            var specFromRoot = _mapSpecification.Map(specification);
            var specFromCommands = _findSpecificationProvider.Create(commands.Select(x => x.EntityInstanceId));
            _childEntity.Recalculate(specFromRoot & specFromCommands);
        }
    }
}