using NuClear.Metamodeling.Elements;

namespace NuClear.River.Common.Metadata.Elements
{
    public interface IStructuralModelTypeElement : IMetadataElement
    {
        StructuralModelTypeKind TypeKind { get; }
    }
}