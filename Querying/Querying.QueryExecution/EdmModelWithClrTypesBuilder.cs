using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
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
        private readonly IDbConnectionFactory _connectionFactory;

        public EdmModelWithClrTypesBuilder(EdmModelBuilder edmModelBuilder, EdmxModelBuilder edmxModelBuilder, IDbConnectionFactory connectionFactory)
        {
            _edmModelBuilder = edmModelBuilder;
            _edmxModelBuilder = edmxModelBuilder;
            _connectionFactory = connectionFactory;
        }

        public IEdmModel Build(Uri uri)
        {
            var connection = _connectionFactory.CreateConnection(uri);
            var edmxModel = _edmxModelBuilder.Build(connection, uri);

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