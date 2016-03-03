using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements;

namespace NuClear.Querying.Edm
{
    public interface IEdmModelAnnotator
    {
        void Annotate(IMetadataElement metadataElement, IEdmElement edmElement, IEdmModel edmModel);
    }
}