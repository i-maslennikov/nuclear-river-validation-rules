using System.Web.OData;

using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm;
using NuClear.Querying.Edm.EF;
using NuClear.Querying.Edm.Emit;
using NuClear.Querying.Metadata.Elements;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    public class EmitEdmModelAnnotator : IEdmModelAnnotator
    {
        private readonly IClrTypeProvider _clrTypeProvider;

        public EmitEdmModelAnnotator(IMetadataProvider metadataProvider)
        {
            var clrTypeResolver = new EmitClrTypeResolver(metadataProvider);
            clrTypeResolver.Build();

            _clrTypeProvider = clrTypeResolver;
        }

        public void Annotate(IMetadataElement metadataElement, IEdmElement edmElement, IEdmModel edmModel)
        {
            if (metadataElement is EntityElement || metadataElement is EnumTypeElement)
            {
                var clrType = _clrTypeProvider.Get(metadataElement.Identity);
                if (clrType != null)
                {
                    edmModel.SetAnnotationValue(edmElement, new ClrTypeAnnotation(clrType));
                }
            }
        }
    }
}