using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Elements
{
    public interface IAggregateMetadata<TEntity>
    {
        MapToObjectsSpecProvider<TEntity, TEntity> MapSpecificationProviderForSource { get; }
        MapToObjectsSpecProvider<TEntity, TEntity> MapSpecificationProviderForTarget { get; }
    }

    // todo: подумать о названии/назначении. Одновременно пытается описать агрегат и сущность.
    public class AggregateMetadata<TEntity, TKey> : MetadataElement<AggregateMetadata<TEntity, TKey>, AggregateMetadataBuilder<TEntity, TKey>>, IAggregateMetadata<TEntity>
        where TEntity : class, IIdentifiable<TKey>
    {
        private IMetadataElementIdentity _identity = new Uri(typeof(TEntity).Name, UriKind.Relative).AsIdentity();

        public AggregateMetadata(
            MapToObjectsSpecProvider<TEntity, TEntity> mapSpecificationProviderForSource,
            MapToObjectsSpecProvider<TEntity, TEntity> mapSpecificationProviderForTarget,
            IEnumerable<IMetadataFeature> features) : base(features)
        {
            MapSpecificationProviderForSource = mapSpecificationProviderForSource;
            MapSpecificationProviderForTarget = mapSpecificationProviderForTarget;
        }

        public override IMetadataElementIdentity Identity => _identity;

        public MapToObjectsSpecProvider<TEntity, TEntity> MapSpecificationProviderForSource { get; }

        public MapToObjectsSpecProvider<TEntity, TEntity> MapSpecificationProviderForTarget { get; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}