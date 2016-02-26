using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.OData;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.Querying.Edm
{
    public sealed class EdmModelBuilder
    {
        private const string DefaultContainerName = "DefaultContainer";
        private const string AnnotationNamespace = "http://schemas.2gis.ru/2015/02/edm/customannotation";
        private const string AnnotationAttribute = "EntityId";

        private readonly IMetadataProvider _metadataProvider;

        public EdmModelBuilder(IMetadataProvider metadataProvider)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException(nameof(metadataProvider));
            }

            _metadataProvider = metadataProvider;
        }

        public IEdmModel Build(Uri contextIdentity, IEnumerable<Type> clrTypes/*, DbCompiledModel dbCompiledModel*/)
        {
            if (contextIdentity == null)
            {
                throw new ArgumentNullException(nameof(contextIdentity));
            }

            BoundedContextElement boundedContextElement;
            _metadataProvider.TryGetMetadata(contextIdentity, out boundedContextElement);
            if (boundedContextElement?.ConceptualModel == null)
            {
                return null;
            }

            var model = Build(boundedContextElement, clrTypes);
            model.SetAnnotationValue(model, new BoundedContextIdentityAnnotation(contextIdentity));
            //model.SetAnnotationValue(model, new DbCompiledModelAnnotation(dbCompiledModel));

            return model;
        }

        private static IEdmModel Build(BoundedContextElement context, IEnumerable<Type> clrTypes)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ConceptualModel == null)
            {
                throw new InvalidOperationException("The conceptual model is not specified.");
            }

            return BuildModel(context.ResolveFullName(), context.ConceptualModel, clrTypes);
        }

        private static IEdmModel BuildModel(string namespaceName, StructuralModelElement conceptualModelElement, IEnumerable<Type> clrTypes)
        {
            var types = clrTypes.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            var model = new EdmModel();

            var container = new EdmEntityContainer(namespaceName, DefaultContainerName);
            model.AddElement(container);

            var typeBuilder = new TypeBuilder(namespaceName);
            BuildEntitySets(container, conceptualModelElement.RootEntities, typeBuilder);

            var enumTypes = conceptualModelElement.Entities.SelectMany(x => x.Properties).Select(x => x.PropertyType).OfType<EnumTypeElement>();
            BuildTypes(model, enumTypes, types, typeBuilder);

            BuildTypes(model, conceptualModelElement.Entities, types, typeBuilder);

            return model;
        }

        private static void BuildEntitySets(EdmEntityContainer container, IEnumerable<EntityElement> rootElements, TypeBuilder typeBuilder)
        {
            foreach (var rootEntity in rootElements)
            {
                var entitySetName = rootEntity.EntitySetName ?? rootEntity.ResolveName();
                var schemaType = typeBuilder.BuildSchemaType(rootEntity);

                container.AddEntitySet(entitySetName, (IEdmEntityType)schemaType);
            }
        }

        private static void BuildTypes(EdmModel model, IEnumerable<IMetadataElement> metadataElements, IDictionary<string, Type> clrTypes, TypeBuilder typeBuilder)
        {
            foreach (var metadataElement in metadataElements)
            {
                var schemaType = typeBuilder.BuildSchemaType(metadataElement);
                RegisterType(model, schemaType, metadataElement.Identity, clrTypes);
            }
        }

        private static void RegisterType(EdmModel model, IEdmSchemaElement schemaElement, IMetadataElementIdentity identity, IDictionary<string, Type> clrTypes)
        {
            model.AddElement(schemaElement);
            model.SetAnnotationValue(schemaElement, AnnotationNamespace, AnnotationAttribute, identity.Id);

            Type clrType;
            if (clrTypes.TryGetValue(schemaElement.Name, out clrType))
            {
                model.SetAnnotationValue(schemaElement, new ClrTypeAnnotation(clrType));
            }
        }

        private sealed class TypeBuilder
        {
            private readonly string _namespaceName;
            private readonly Dictionary<IMetadataElementIdentity, IEdmSchemaType> _builtTypes;

            public TypeBuilder(string namespaceName)
            {
                _namespaceName = namespaceName;
                _builtTypes = new Dictionary<IMetadataElementIdentity, IEdmSchemaType>();
            }

            public IEdmSchemaType BuildSchemaType(IMetadataElement metadataElement)
            {
                IEdmSchemaType complexType;
                if (!_builtTypes.TryGetValue(metadataElement.Identity, out complexType))
                {
                    var entityElement = metadataElement as EntityElement;
                    if (entityElement != null)
                    {
                        _builtTypes.Add(metadataElement.Identity,
                                        complexType = entityElement.KeyProperties.Any()
                                                          ? (IEdmSchemaType)BuildEntityType(entityElement)
                                                          : (IEdmSchemaType)BuildComplexType(entityElement));

                    }
                }

                return complexType;
            }

            private IEdmEntityType BuildEntityType(EntityElement entityElement)
            {
                var entityType = new EdmEntityType(_namespaceName, entityElement.ResolveName());
                var keyIds = new HashSet<IMetadataElementIdentity>(entityElement.KeyProperties.Select(x => x.Identity));

                foreach (var propertyElement in entityElement.Properties)
                {
                    var propertyName = propertyElement.ResolveName();
                    var propertyType = BuildPropertyType(propertyElement);

                    var property = entityType.AddStructuralProperty(propertyName, propertyType);
                    if (keyIds.Contains(propertyElement.Identity))
                    {
                        entityType.AddKeys(property);
                    }
                }

                foreach (var relationElement in entityElement.Relations)
                {
                    var propertyName = relationElement.ResolveName();
                    var structuredType = BuildSchemaType(relationElement.Target);

                    if (structuredType is IEdmComplexType)
                    {
                        var typeReference = BuildTypeReference(relationElement);
                        entityType.AddStructuralProperty(propertyName, typeReference);
                    }

                    var relatedEntityType = structuredType as IEdmEntityType;
                    if (relatedEntityType != null)
                    {
                        entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
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

            private IEdmComplexType BuildComplexType(EntityElement entityElement)
            {
                var entityType = new EdmComplexType(_namespaceName, entityElement.ResolveName());

                foreach (var propertyElement in entityElement.Properties)
                {
                    var propertyName = propertyElement.ResolveName();
                    var propertyType = BuildPropertyType(propertyElement);

                    entityType.AddStructuralProperty(propertyName, propertyType);
                }

                foreach (var relationElement in entityElement.Relations)
                {
                    var propertyName = relationElement.ResolveName();
                    var structuredType = BuildSchemaType(relationElement.Target);

                    if (structuredType is IEdmComplexType)
                    {
                        var typeReference = BuildTypeReference(relationElement);
                        entityType.AddStructuralProperty(propertyName, typeReference);
                    }
                }

                return entityType;
            }

            private IEdmTypeReference BuildPropertyType(EntityPropertyElement propertyElement)
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
                    return new EdmEnumTypeReference(BuildEnumType(enumType), propertyElement.IsNullable);
                }

                throw new NotSupportedException();
            }

            private IEdmTypeReference BuildTypeReference(EntityRelationElement relationElement)
            {
                var complexType = (IEdmComplexType)BuildSchemaType(relationElement.Target);

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

            private IEdmEnumType BuildEnumType(EnumTypeElement enumTypeElement)
            {
                IEdmSchemaType enumType;
                if (!_builtTypes.TryGetValue(enumTypeElement.Identity, out enumType))
                {
                    _builtTypes.Add(enumTypeElement.Identity, enumType = BuildEdmEnumType(enumTypeElement));
                }

                return (IEdmEnumType)enumType;
            }


            private IEdmEnumType BuildEdmEnumType(EnumTypeElement enumTypeElement)
            {
                var enumType = new EdmEnumType(_namespaceName, enumTypeElement.ResolveName(), Convert(enumTypeElement.UnderlyingType), false);

                foreach (var member in enumTypeElement.Members)
                {
                    enumType.AddMember(member.Key, new EdmIntegerConstant(member.Value));
                }

                return enumType;
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
                        throw new ArgumentOutOfRangeException(nameof(typeKind));
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
                        throw new ArgumentOutOfRangeException(nameof(cardinality));
                }
            }
        }

        private sealed class DbCompiledModelAnnotation
        {
            public DbCompiledModelAnnotation(DbCompiledModel value)
            {
                Value = value;
            }

            public DbCompiledModel Value { get; }
        }
    }
}
