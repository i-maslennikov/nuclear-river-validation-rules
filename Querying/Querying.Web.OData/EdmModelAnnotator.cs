using System.Web.OData;

using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements;
using NuClear.Querying.Edm;
using NuClear.Querying.Edm.EF;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Querying.Web.OData
{
    public class EdmModelAnnotator : IEdmModelAnnotator
    {
        private const string AnnotationNamespace = "http://schemas.2gis.ru/2015/02/edm/customannotation";
        private const string AnnotationAttribute = "EntityId";

        private readonly IClrTypeProvider _clrTypeProvider;

        public EdmModelAnnotator(IClrTypeProvider clrTypeProvider)
        {
            _clrTypeProvider = clrTypeProvider;
        }

        public void Annotate(IMetadataElement metadataElement, IEdmElement edmElement, IEdmModel edmModel)
        {
            if (metadataElement is EntityElement || metadataElement is EnumTypeElement)
            {
                edmModel.SetAnnotationValue(edmElement, AnnotationNamespace, AnnotationAttribute, metadataElement.Identity.Id);

                var clrType = _clrTypeProvider.Get(metadataElement.Identity);
                if (clrType != null)
                {
                    edmModel.SetAnnotationValue(edmElement, new ClrTypeAnnotation(clrType));
                }
            }
        }
    }
}