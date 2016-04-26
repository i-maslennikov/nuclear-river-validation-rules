using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Querying.Metadata.Identities;

namespace NuClear.Querying.Metadata.Elements
{
    public sealed class PrimitiveTypeElement : MetadataElement, IStructuralModelTypeElement
    {
        private static readonly IReadOnlyDictionary<ElementaryTypeKind, PrimitiveTypeElement> PrimitiveTypes =
            Enum.GetValues(typeof(ElementaryTypeKind)).Cast<ElementaryTypeKind>().ToDictionary(x => x, x => new PrimitiveTypeElement(x));

        private readonly IMetadataElementIdentity _identity;

        private PrimitiveTypeElement(ElementaryTypeKind elementaryTypeKind)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            Uri uri = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<QueryingMetadataIdentity>(elementaryTypeKind.ToString());
            _identity = uri.AsIdentity();
            PrimitiveType = elementaryTypeKind;
        }

        public static PrimitiveTypeElement OfKind(ElementaryTypeKind elementaryTypeKind)
        {
            PrimitiveTypeElement element;
            if (!PrimitiveTypes.TryGetValue(elementaryTypeKind, out element))
            {
                throw new NotSupportedException("The specified primitive type is not supported.");
            }
            return element;
        }

        public StructuralModelTypeKind TypeKind
        {
            get
            {
                return StructuralModelTypeKind.Primitive;
            }
        }

        public ElementaryTypeKind PrimitiveType { get; private set; }

        public override IMetadataElementIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
        }
    }
}