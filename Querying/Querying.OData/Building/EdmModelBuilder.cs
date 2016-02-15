using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Querying.OData.Building
{
    public sealed class EdmModelBuilder
    {
        private const string DefaultContainName = "DefaultContainer";

        private readonly IMetadataProvider _metadataProvider;

        public EdmModelBuilder(IMetadataProvider metadataProvider)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException("metadataProvider");
            }
            _metadataProvider = metadataProvider;
        }

        public IEdmModel Build(Uri contextUrl)
        {
            if (contextUrl == null)
            {
                throw new ArgumentNullException("contextUrl");
            }

            BoundedContextElement boundedContextElement;
            _metadataProvider.TryGetMetadata(contextUrl, out boundedContextElement);
            if (boundedContextElement == null || boundedContextElement.ConceptualModel == null)
            {
                return null;
            }

            return Build(boundedContextElement);
        }

        private static IEdmModel Build(BoundedContextElement context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.ConceptualModel == null)
            {
                throw new InvalidOperationException("The conceptual model is not specified.");
            }

            return BuildModel(context.ResolveFullName(), context.ConceptualModel.RootEntities);
        }

        private static IEdmModel BuildModel(string namespaceName, IEnumerable<EntityElement> entities)
        {
            var model = new EdmModel();

            var typeBuilder = new TypeBuilder(namespaceName, model.DirectValueAnnotationsManager);

            model.AddElement(BuildContainer(entities, typeBuilder));

            foreach (var registeredType in typeBuilder.RegisteredTypes)
            {
                model.AddElement(registeredType);
            }

            return model;
        }

        private static IEdmEntityContainer BuildContainer(IEnumerable<EntityElement> entities, TypeBuilder typeBuilder)
        {
            var container = new EdmEntityContainer(typeBuilder.NamespaceName, DefaultContainName);

            foreach (var entityElement in entities)
            {
                var entitySetName = entityElement.EntitySetName ?? entityElement.ResolveName();
                var entityType = (IEdmEntityType)typeBuilder.ResolveComplexType(entityElement);

                container.AddEntitySet(entitySetName, entityType);
            }

            return container;
        }

        #region TypeBuilder

        private class TypeBuilder
        {
            private const string AnnotationNamespace = "http://schemas.2gis.ru/2015/02/edm/customannotation";
            private const string AnnotationAttribute = "EntityId";

            private readonly IEdmDirectValueAnnotationsManager _annotationsManager;
            private readonly Dictionary<string, IEdmSchemaType> _registeredTypes;

            public TypeBuilder(string namespaceName, IEdmDirectValueAnnotationsManager annotationsManager)
            {
                NamespaceName = namespaceName;
                _annotationsManager = annotationsManager;
                _registeredTypes = new Dictionary<string, IEdmSchemaType>();
            }

            public string NamespaceName { get; private set; }

            public IEnumerable<IEdmSchemaType> RegisteredTypes
            {
                get
                {
                    return _registeredTypes.Values;
                }
            }

            public IEdmStructuredType ResolveComplexType(EntityElement entityElement)
            {
                var typeName = entityElement.ResolveName();

                IEdmSchemaType complexType;
                if (!_registeredTypes.TryGetValue(typeName, out complexType))
                {
                    _registeredTypes.Add(typeName,
                        complexType = entityElement.KeyProperties.Any()
                        ? (IEdmSchemaType)BuildEntityType(typeName, entityElement)
                        : (IEdmSchemaType)BuildComplexType(typeName, entityElement));

                    AnnotateElement(complexType, entityElement.Identity.Id);
                }

                return (IEdmStructuredType)complexType;
            }

            private IEdmEnumType ResolveEnumType(EnumTypeElement enumTypeElement)
            {
                var typeName = enumTypeElement.ResolveName();

                IEdmSchemaType enumType;
                if (!_registeredTypes.TryGetValue(typeName, out enumType))
                {
                    _registeredTypes.Add(typeName, enumType = BuildEnumType(typeName, enumTypeElement));
                    
                    AnnotateElement(enumType, enumTypeElement.Identity.Id);
                }

                return (IEdmEnumType)enumType;
            }

            private IEdmTypeReference ResolveTypeReference(EntityPropertyElement propertyElement)
            {
                var propertyType = propertyElement.PropertyType;

                var primitiveType = propertyType as PrimitiveTypeElement;
                if (primitiveType != null)
                {
                    return EdmCoreModel.Instance.GetPrimitive(Convert(primitiveType.PrimitiveType), propertyElement.IsNullable);
                }

                var enumType = propertyType as EnumTypeElement;
                if (enumType != null)
                {
                    return new EdmEnumTypeReference(ResolveEnumType(enumType), propertyElement.IsNullable);
                }

                throw new NotSupportedException();
            }

            private IEdmTypeReference ResolveTypeReference(EntityRelationElement relationElement)
            {
                var complexType = (IEdmComplexType)ResolveComplexType(relationElement.Target);

                IEdmTypeReference typeReference;
                switch (relationElement.Cardinality)
                {
                    case EntityRelationCardinality.One:
                        typeReference = new EdmComplexTypeReference(complexType, false);
                        break;
                    case EntityRelationCardinality.OptionalOne:
                        typeReference = new EdmComplexTypeReference(complexType, true);
                        break;
                    case EntityRelationCardinality.Many:
                        typeReference = EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, true));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return typeReference;
            }

            private IEdmEntityType BuildEntityType(string typeName, EntityElement entityElement)
            {
                var entityType = new EdmEntityType(NamespaceName, typeName);
                var keyIds = new HashSet<IMetadataElementIdentity>(entityElement.KeyProperties.Select(x => x.Identity));
                
                foreach (var propertyElement in entityElement.Properties)
                {
                    var propertyName = propertyElement.ResolveName();
                    var typeReference = ResolveTypeReference(propertyElement);

                    var property = entityType.AddStructuralProperty(propertyName, typeReference);
                    if (keyIds.Contains(propertyElement.Identity))
                    {
                        entityType.AddKeys(property);
                    }
                }

                foreach (var relationElement in entityElement.Relations)
                {
                    var propertyName = relationElement.ResolveName();
                    var structuredType = ResolveComplexType(relationElement.Target);

                    if (structuredType is IEdmComplexType)
                    {
                        var typeReference = ResolveTypeReference(relationElement);
                        entityType.AddStructuralProperty(propertyName, typeReference);
                    }

                    var relatedEntityType = structuredType as IEdmEntityType;
                    if (relatedEntityType != null)
                    {

                        entityType.AddUnidirectionalNavigation(
                                                               new EdmNavigationPropertyInfo
                                                               {
                                                                   Name = propertyName,
                                                                   ContainsTarget = relationElement.ContainsTarget,
                                                                   Target = relatedEntityType,
                                                                   TargetMultiplicity = Convert(relationElement.Cardinality)
                                                               });
                    }
                }

                return entityType;
            }

            private IEdmComplexType BuildComplexType(string typeName, EntityElement entityElement)
            {
                var entityType = new EdmComplexType(NamespaceName, typeName);

                foreach (var propertyElement in entityElement.Properties)
                {
                    var propertyName = propertyElement.ResolveName();
                    var typeReference = ResolveTypeReference(propertyElement);

                    entityType.AddStructuralProperty(propertyName, typeReference);
                }

                foreach (var relationElement in entityElement.Relations)
                {
                    var propertyName = relationElement.ResolveName();
                    var structuredType = ResolveComplexType(relationElement.Target);

                    if (structuredType is IEdmComplexType)
                    {
                        var typeReference = ResolveTypeReference(relationElement);
                        entityType.AddStructuralProperty(propertyName, typeReference);
                    }
                }

                return entityType;
            }

            private IEdmEnumType BuildEnumType(string typeName, EnumTypeElement enumTypeElement)
            {
                var enumType = new EdmEnumType(NamespaceName, typeName, Convert(enumTypeElement.UnderlyingType), false);

                foreach (var member in enumTypeElement.Members)
                {
                    enumType.AddMember(member.Key, new EdmIntegerConstant(member.Value));
                }

                return enumType;
            }

            private void AnnotateElement(IEdmSchemaType schemaElement, Uri elementId)
            {
                _annotationsManager.SetAnnotationValue(schemaElement, AnnotationNamespace, AnnotationAttribute, elementId);
            }

            private static EdmPrimitiveTypeKind Convert(ElementaryTypeKind typeKind)
            {
                switch (typeKind)
                {
                    case ElementaryTypeKind.Boolean:
                        return EdmPrimitiveTypeKind.Boolean;

                    case ElementaryTypeKind.Byte:
                        return EdmPrimitiveTypeKind.Byte;
                    case ElementaryTypeKind.Int16:
                        return EdmPrimitiveTypeKind.Int16;
                    case ElementaryTypeKind.Int32:
                        return EdmPrimitiveTypeKind.Int32;
                    case ElementaryTypeKind.Int64:
                        return EdmPrimitiveTypeKind.Int64;

                    case ElementaryTypeKind.Single:
                        return EdmPrimitiveTypeKind.Single;
                    case ElementaryTypeKind.Double:
                        return EdmPrimitiveTypeKind.Double;
                    case ElementaryTypeKind.Decimal:
                        return EdmPrimitiveTypeKind.Decimal;

                    case ElementaryTypeKind.DateTimeOffset:
                        return EdmPrimitiveTypeKind.DateTimeOffset;

                    case ElementaryTypeKind.String:
                        return EdmPrimitiveTypeKind.String;

                    default:
                        throw new ArgumentOutOfRangeException("typeKind");
                }
            }

            private static EdmMultiplicity Convert(EntityRelationCardinality cardinality)
            {
                switch (cardinality)
                {
                    case EntityRelationCardinality.One:
                        return EdmMultiplicity.One;
                    case EntityRelationCardinality.OptionalOne:
                        return EdmMultiplicity.ZeroOrOne;
                    case EntityRelationCardinality.Many:
                        return EdmMultiplicity.Many;
                    default:
                        throw new ArgumentOutOfRangeException("cardinality");
                }
            }
        }

        #endregion
    }
}
