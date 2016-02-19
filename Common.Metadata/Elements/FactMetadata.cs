using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Builders;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public class FactMetadata<T> : MetadataElement<FactMetadata<T>, FactMetadataBuilder<T>> 
        where T : class, IIdentifiable<DefaultIdentity, long>
    {
        private IMetadataElementIdentity _identity = new Uri(typeof(T).Name, UriKind.Relative).AsIdentity();

        public FactMetadata(
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource,
            MapToObjectsSpecProvider<T, T> mapSpecificationProviderForTarget,
            Func<IReadOnlyCollection<long>, FindSpecification<T>> findSpecificationProvider,
            IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            MapSpecificationProviderForSource = mapSpecificationProviderForSource;
            MapSpecificationProviderForTarget = mapSpecificationProviderForTarget;
            FindSpecificationProvider = findSpecificationProvider;
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<T, T> MapSpecificationProviderForTarget { get; private set; }

        public Func<IReadOnlyCollection<long>, FindSpecification<T>> FindSpecificationProvider { get; private set; }
    }
}