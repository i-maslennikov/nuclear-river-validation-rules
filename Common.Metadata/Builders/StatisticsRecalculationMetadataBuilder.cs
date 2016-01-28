using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Builders
{
    public class StatisticsRecalculationMetadataBuilder<T> : MetadataElementBuilder<StatisticsRecalculationMetadataBuilder<T>, StatisticsRecalculationMetadata<T>>
    {
        private MapToObjectsSpecProvider<T, T> _mapSpecificationProviderForSource;
        private MapToObjectsSpecProvider<T, T> _mapSpecificationProviderForTarget;
        private Func<long, IReadOnlyCollection<long>, FindSpecification<T>> _findSpecificationProvider;

        protected override StatisticsRecalculationMetadata<T> Create()
        {
            return new StatisticsRecalculationMetadata<T>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                _findSpecificationProvider,
                Features);
        }

        public StatisticsRecalculationMetadataBuilder<T> HasSource(MapSpecification<IQuery, IQueryable<T>> sourceMappingSpecification)
        {
            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<T>>(q => sourceMappingSpecification.Map(q).Where(specification));
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<T> HasTarget(MapSpecification<IQuery, IQueryable<T>> targetMappingSpecification)
        {
            _mapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification)); ;
            return this;
        }

        public StatisticsRecalculationMetadataBuilder<T> HasFilter(Func<long, IReadOnlyCollection<long>, FindSpecification<T>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }
    }
}