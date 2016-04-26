using NuClear.Metamodeling.Elements;

namespace NuClear.Querying.Metadata.Elements
{
    public interface IStructuralModelTypeElement : IMetadataElement
    {
        StructuralModelTypeKind TypeKind { get; }
    }
}