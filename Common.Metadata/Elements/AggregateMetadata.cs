using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public class AggregateMetadata<T, TKey> : MetadataElement<AggregateMetadata<T, TKey>, AggregateMetadataBuilder<T, TKey>>
        where T : class, IIdentifiable<TKey>
    {
        private IMetadataElementIdentity _identity = new Uri(typeof(T).Name, UriKind.Relative).AsIdentity();

        public AggregateMetadata(
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource,
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget,
            IIdentityProvider<TKey> identityProvider,
            IEnumerable<IMetadataFeature> features) : base(features)
        {
            MapSpecificationProviderForSource = mapSpecificationProviderForSource;
            MapSpecificationProviderForTarget = mapSpecificationProviderForTarget;
            FindSpecificationProvider = keys => new FindSpecification<T>(identityProvider.Create<T, TKey>(keys));
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; private set; }

        public Func<IReadOnlyCollection<TKey>, FindSpecification<T>> FindSpecificationProvider { get; private set; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}