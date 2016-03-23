using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
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

        // FIXME {all, 03.09.2015}: TEntity заменить на идентификатор типа
        public FactMetadataBuilder<TFact> HasDependentEntity<TEntity, TKey>(MapToObjectsSpecProvider<TFact, TKey> dependentEntitySpecProvider)
            where TEntity : class, IIdentifiable<TKey>
        {
            AddFeatures(new IndirectlyDependentEntityFeature<TFact, TKey>(typeof(TEntity), dependentEntitySpecProvider));
            return this;
        }

        // FIXME {all, 03.09.2015}: TEntity заменить на идентификатор типа
        public FactMetadataBuilder<TFact> HasMatchedEntity<TEntity>()
            where TEntity : class, IIdentifiable<long>
        {
            AddFeatures(new DirectlyDependentEntityFeature<TFact>(typeof(TEntity)));
            return this;
        }
    }
}