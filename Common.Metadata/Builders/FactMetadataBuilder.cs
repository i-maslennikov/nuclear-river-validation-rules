using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Builders
{
    public class FactMetadataBuilder<TFact> : MetadataElementBuilder<FactMetadataBuilder<TFact>, FactMetadata<TFact>>
        where TFact : class, IIdentifiable<long>
    {
        private MapSpecification<IQuery, IQueryable<TFact>> _sourceMappingSpecification;

        protected override FactMetadata<TFact> Create()
        {
            MapToObjectsSpecProvider<TFact, TFact> mapSpecificationProviderForSource =
                specification => new MapSpecification<IQuery, IEnumerable<TFact>>(q => _sourceMappingSpecification.Map(q).Where(specification));

            var targetMappingSpecification = new MapSpecification<IQuery, IQueryable<TFact>>(q => q.For<TFact>());
            MapToObjectsSpecProvider<TFact, TFact> mapSpecificationProviderForTarget = 
                specification => new MapSpecification<IQuery, IEnumerable<TFact>>(q => targetMappingSpecification.Map(q).Where(specification));

            return new FactMetadata<TFact>(mapSpecificationProviderForSource, mapSpecificationProviderForTarget, Features);
        }

        public FactMetadataBuilder<TFact> HasSource(MapSpecification<IQuery, IQueryable<TFact>> sourceMappingSpecification)
        {
            _sourceMappingSpecification = sourceMappingSpecification;
            return this;
        }

        public FactMetadataBuilder<TFact> HasDependentEntity<TKey>(IEntityType entityType, MapToObjectsSpecProvider<TFact, TKey> dependentAggregateSpecProvider)
        {
            AddFeatures(new IndirectlyDependentEntityFeature<TFact, TKey>(entityType, dependentAggregateSpecProvider));
            return this;
        }

        public FactMetadataBuilder<TFact> HasMatchedEntity(IEntityType entityType)
        {
            AddFeatures(new DirectlyDependentEntityFeature<TFact>(entityType));
            return this;
        }
    }
}