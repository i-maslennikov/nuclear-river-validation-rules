using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements;
using NuClear.Querying.Edm;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public class NullEdmModelAnnotator : IEdmModelAnnotator
    {
        public void Annotate(IMetadataElement metadataElement, IEdmElement edmElement, IEdmModel edmModel)
        {
        }
    }
}