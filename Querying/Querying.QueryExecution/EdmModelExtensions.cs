using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;

namespace NuClear.Querying.QueryExecution
{
    public static class EdmModelExtensions
    {
        private const string AnnotationNamespace = "http://schemas.2gis.ru/2015/02/edm/customannotation";
        private const string AnnotationAttribute = "EntityId";

        public static IEdmModel AnnotateByClrTypes(this IEdmModel model, IEnumerable<Type> clrTypes)
        {
            var tuples = model.SchemaElements.Join(clrTypes, x => x.Name, x => x.Name, Tuple.Create, StringComparer.OrdinalIgnoreCase);

            foreach (var tuple in tuples)
            {
                var annotation = new ClrTypeAnnotation(tuple.Item2);
                model.SetAnnotationValue(tuple.Item1, annotation);
            }

            return model;
        }

        public static IEdmModel AnnotateByClrTypes(this IEdmModel model, Func<Uri,Type> resolver)
        {
            foreach (var element in model.SchemaElements)
            {
                var entityId = model.GetAnnotationValue<Uri>(element, AnnotationNamespace, AnnotationAttribute);
                if (entityId == null)
                {
                    continue;
                }

                var type = resolver(entityId);
                if (type == null)
                {
                    continue;
                }

                model.SetAnnotationValue(element, new ClrTypeAnnotation(type));
            }

            return model;
        }

        public static IEdmModel AddDbCompiledModelAnnotation(this IEdmModel model, DbCompiledModel dbCompiledModel)
        {
            var annotation = new DbCompiledModelAnnotation(dbCompiledModel);
            model.SetAnnotationValue(model, annotation);

            return model;
        }

        public static DbCompiledModel GetDbCompiledModel(this IEdmModel model)
        {
            var annotation = model.GetAnnotationValue<DbCompiledModelAnnotation>(model);
            return annotation.Value;
        }


        public static IEdmModel AddMetadataIdentityAnnotation(this IEdmModel model, Uri identity)
        {
            var annotation = new MetadataIdentityAnnotation(identity);
            model.SetAnnotationValue(model, annotation);

            return model;
        }

        public static Uri GetMetadataIdentity(this IEdmModel model)
        {
            var annotation = model.GetAnnotationValue<MetadataIdentityAnnotation>(model);
            return annotation.Value;
        }

        public static string ResolveName(this Uri id)
        {
            return id.ResolvePath().Split('/').LastOrDefault();
        }

        private static string ResolvePath(this Uri id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return id.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        }

    }
}