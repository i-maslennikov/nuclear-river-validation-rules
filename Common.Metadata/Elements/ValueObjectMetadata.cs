using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public class ValueObjectMetadata<TValueObject, TEntityKey> : MetadataElement, IValueObjectMetadata
    {
        private IMetadataElementIdentity _identity = new Uri(typeof(TValueObject).Name, UriKind.Relative).AsIdentity();

        public ValueObjectMetadata(
            MapToObjectsSpecProvider<TValueObject, TValueObject> mapSpecificationProviderForSource,
            MapToObjectsSpecProvider<TValueObject, TValueObject> mapSpecificationProviderForTarget,
            Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TValueObject>> findSpecificationProvider)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            MapSpecificationProviderForSource = mapSpecificationProviderForSource;
            MapSpecificationProviderForTarget = mapSpecificationProviderForTarget;
            FindSpecificationProvider = findSpecificationProvider;
        }

        public override IMetadataElementIdentity Identity => _identity;

        public Type ValueObjectType => typeof(TValueObject);

        public Type EntityKeyType => typeof(TEntityKey);

        public MapToObjectsSpecProvider<TValueObject, TValueObject> MapSpecificationProviderForSource { get; private set; }

        public MapToObjectsSpecProvider<TValueObject, TValueObject> MapSpecificationProviderForTarget { get; private set; }

        // todo: подумать о замене TEntityKey на TEntity
        public Func<IReadOnlyCollection<TEntityKey>, FindSpecification<TValueObject>> FindSpecificationProvider { get; private set; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }
    }
}