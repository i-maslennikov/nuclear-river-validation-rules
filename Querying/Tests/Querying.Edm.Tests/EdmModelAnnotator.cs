using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;

using NuClear.Metamodeling.Elements;
using NuClear.Querying.Metadata.Elements;

namespace NuClear.Querying.Edm.Tests
{
    public class EdmModelAnnotator : IEdmModelAnnotator
    {
        private readonly IEnumerable<Type> _clrTypes;

        public EdmModelAnnotator(IEnumerable<Type> clrTypes)
        {
            _clrTypes = clrTypes;
        }

        public void Annotate(IMetadataElement metadataElement, IEdmElement edmElement, IEdmModel edmModel)
        {
            if (metadataElement is EntityElement || metadataElement is EnumTypeElement)
            {
                var typeName = metadataElement.Identity.Id.Segments.Last();
                var clrType = _clrTypes.SingleOrDefault(x => x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                if (clrType != null)
                {
                    edmModel.SetAnnotationValue(edmElement, new ClrTypeAnnotation(clrType));
                }
            }
        }
    }
}