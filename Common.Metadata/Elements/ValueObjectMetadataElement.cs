using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public class ValueObjectMetadataElement<T> : MetadataElement, IValueObjectMetadataElement
    {
        private IMetadataElementIdentity _identity = new Uri(typeof(T).Name, UriKind.Relative).AsIdentity();

        public ValueObjectMetadataElement(
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource,
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget,
            Func<IReadOnlyCollection<long>, FindSpecification<T>> findSpecificationProvider)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            MapSpecificationProviderForSource = mapSpecificationProviderForSource;
            MapSpecificationProviderForTarget = mapSpecificationProviderForTarget;
            FindSpecificationProvider = findSpecificationProvider;
        }

        public override IMetadataElementIdentity Identity => _identity;

        public Type ValueObjectType => typeof(T);

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; private set; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}