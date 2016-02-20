using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;

namespace NuClear.River.Common.Metadata.Elements
{
    public sealed class EnumTypeElement : BaseMetadataElement<EnumTypeElement, EnumTypeElementBuilder>, IStructuralModelTypeElement
    {
        internal EnumTypeElement(IMetadataElementIdentity identity, IReadOnlyDictionary<string, long> members, ElementaryTypeKind typeKind = ElementaryTypeKind.Int32)
            : base(identity, Enumerable.Empty<IMetadataFeature>())
        {
            Members = members;
            UnderlyingType = typeKind;
        }

        public StructuralModelTypeKind TypeKind
        {
            get
            {
                return StructuralModelTypeKind.Enum;
            }
        }

        public ElementaryTypeKind UnderlyingType { get; private set; }

        public IReadOnlyDictionary<string, long> Members { get; private set; }
    }
}