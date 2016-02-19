using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Builders
{
    public class AggregateMetadataBuilder<T> : MetadataElementBuilder<AggregateMetadataBuilder<T>, AggregateMetadata<T>> where T : class, IIdentifiable<DefaultIdentity, long>
    {
        private MapSpecification<IQuery, IQueryable<T>> _mapToSourceSpec;

        protected override AggregateMetadata<T> Create()
        {
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource = 
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => _mapToSourceSpec.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<T>>(q => q.For<T>());
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget =
                specification => new MapSpecification<IQuery, IEnumerable<T>>(q => targetMappingSpecification.Map(q).Where(specification));

            return new AggregateMetadata<T>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, Specs.Find.ByIds<T>, Features);
        }

        public AggregateMetadataBuilder<T> HasSource(MapSpecification<IQuery, IQueryable<T>> mapToSourceSpec)
        {
            _mapToSourceSpec = mapToSourceSpec;
            return this;
        }

        public AggregateMetadataBuilder<T> HasValueObject<TValueObject>(
            MapSpecification<IQuery, IQueryable<TValueObject>> sourceMappingSpecification,
            Func<IReadOnlyCollection<long>, FindSpecification<TValueObject>> findSpecificationProvider)
            where TValueObject : class
        {
            MapToObjectsSpecProvider<TValueObject, TValueObject> mapSpecificationProviderForSource = 
                specification => new MapSpecification<IQuery, IEnumerable<TValueObject>>(q => sourceMappingSpecification.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TValueObject>>(q => q.For<TValueObject>());
            MapToObjectsSpecProvider<TValueObject, TValueObject> mapSpecificationProviderForTarget = 
                specification => new MapSpecification<IQuery, IEnumerable<TValueObject>>(q => targetMappingSpecification.Map(q).Where(specification));
            
            Childs(new ValueObjectMetadataElement<TValueObject>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, findSpecificationProvider));
            return this;
        }
    }
}