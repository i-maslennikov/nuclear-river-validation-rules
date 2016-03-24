using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Builders
{
    public class StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey> : MetadataElementBuilder<StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey>, StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey>>
    {
        private MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> _mapSpecificationProviderForSource;
        private MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> _mapSpecificationProviderForTarget;
        private Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TStatisticsObject>> _findSpecificationProvider;

        protected override StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey> Create()
        {
            return new StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                _findSpecificationProvider);
        }

        public StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey> HasSource(MapSpecification<IQuery, IQueryable<TStatisticsObject>> sourceMappingSpecification)
        {
            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<TStatisticsObject>>(q => sourceMappingSpecification.Map(q).Where(specification));
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey> HasTarget(MapSpecification<IQuery, IQueryable<TStatisticsObject>> targetMappingSpecification)
        {
            _mapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<TStatisticsObject>>(q => targetMappingSpecification.Map(q).Where(specification)); ;
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey> HasFilter(Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TStatisticsObject>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }
    }
}