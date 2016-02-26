using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Microsoft.OData.Edm;

using NuClear.Querying.Edm.Edmx;

namespace NuClear.Querying.Edm
{
    public sealed class EdmModelWithClrTypesBuilder
    {
        private const string ClrTypeAnnotation = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType";

        private readonly EdmModelBuilder _edmModelBuilder;
        private readonly EdmxModelBuilder _edmxModelBuilder;

        public EdmModelWithClrTypesBuilder(EdmModelBuilder edmModelBuilder, EdmxModelBuilder edmxModelBuilder)
        {
            _edmModelBuilder = edmModelBuilder;
            _edmxModelBuilder = edmxModelBuilder;
        }

        public IEdmModel Build(Uri uri, DbProviderInfo providerInfo)
        {
            var edmxModel = _edmxModelBuilder.Build(uri, providerInfo);

            return _edmModelBuilder.Build(uri, GetClrTypes(edmxModel.ConceptualModel)/*, edmxModel.Compile()*/);
        }

        private static IEnumerable<Type> GetClrTypes(EdmModel model)
        {
            var metadataItems = model.EntityTypes.Cast<MetadataItem>().Union(model.ComplexTypes).Union(model.EnumTypes);
            var clrTypes = metadataItems
                .SelectMany(x => x.MetadataProperties)
                .Where(x => x.IsAnnotation && x.Name.Equals(ClrTypeAnnotation, StringComparison.Ordinal))
                .Select(x => (Type)x.Value);

            return clrTypes;
        }
    }
}