using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Builders
{
    public class AggregateMetadataBuilder<T, TKey> : MetadataElementBuilder<AggregateMetadataBuilder<T, TKey>, AggregateMetadata<T, TKey>>
        where T : class, IIdentifiable<TKey>
    {
        private MapSpecification<IQuery, IQueryable<T>> _mapToSourceSpec;

        protected override AggregateMetadata<T, TKey> Create()
        {
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource = 
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => _mapToSourceSpec.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<T>>(q => q.For<T>());
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget =
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification));

            return new AggregateMetadata<T, TKey>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, Features);
        }

        public AggregateMetadataBuilder<T, TKey> HasSource(MapSpecification<IQuery, IQueryable<T>> mapToSourceSpec)
        {
            _mapToSourceSpec = mapToSourceSpec;
            return this;
        }

        public AggregateMetadataBuilder<T, TKey> HasValueObject<TValueObject>(
            MapSpecification<IQuery, IQueryable<TValueObject>> sourceMappingSpecification,
            Func<IReadOnlyCollection<TKey>, FindSpecification<TValueObject>> findSpecificationProvider)
            where TValueObject : class
        {
            MapToObjectsSpecProvider<TValueObject, TValueObject> mapSpecificationProviderForSource = 
                specification => new MapSpecification<IQuery, IEnumerable<TValueObject>>(q => sourceMappingSpecification.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TValueObject>>(q => q.For<TValueObject>());
            MapToObjectsSpecProvider<TValueObject, TValueObject> mapSpecificationProviderForTarget = 
                specification => new MapSpecification<IQuery, IEnumerable<TValueObject>>(q => targetMappingSpecification.Map(q).Where(specification));

            Childs(new ValueObjectMetadata<TValueObject, TKey>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, findSpecificationProvider));
            return this;
        }
    }
}