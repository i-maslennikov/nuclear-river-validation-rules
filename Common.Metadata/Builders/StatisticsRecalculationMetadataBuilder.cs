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
        where TStatisticsObject : class
    {
        private MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> _mapSpecificationProviderForSource;
        private Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TStatisticsObject>> _findSpecificationProvider;

        protected override StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey> Create()
        {
            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TStatisticsObject>>(q => q.For<TStatisticsObject>());
            MapToObjectsSpecProvider<TStatisticsObject, TStatisticsObject> mapSpecificationProviderForTarget =
                specification => new MapSpecification<IQuery, IEnumerable<TStatisticsObject>>(q => targetMappingSpecification.Map(q).Where(specification));

            return new StatisticsRecalculationMetadata<TStatisticsObject, TEntityKey>(
                _mapSpecificationProviderForSource,
                mapSpecificationProviderForTarget,
                _findSpecificationProvider);
        }

        public StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey> HasSource(MapSpecification<IQuery, IQueryable<TStatisticsObject>> sourceMappingSpecification)
        {
            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<TStatisticsObject>>(q => sourceMappingSpecification.Map(q).Where(specification));
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<TStatisticsObject, TEntityKey> HasFilter(Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TStatisticsObject>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }
    }
}