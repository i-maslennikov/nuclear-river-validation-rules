using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements;

namespace NuClear.Querying.Edm.Tests.Edm
{
    public class NullEdmModelAnnotator : IEdmModelAnnotator
    {
        public void Annotate(IMetadataElement metadataElement, IEdmElement edmElement, IEdmModel edmModel)
        {
        }
    }
}