using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.Querying.Edm
{
    public sealed class EdmModelBuilder : IEdmModelBuilder
    {
        private const string DefaultContainerName = "DefaultContainer";

        private readonly IMetadataProvider _metadataProvider;
        private readonly IEdmModelAnnotator _edmModelAnnotator;

        public EdmModelBuilder(IMetadataProvider metadataProvider, IEdmModelAnnotator edmModelAnnotator)
        {
            _metadataProvider = metadataProvider;
            _edmModelAnnotator = edmModelAnnotator;
        }

        public IReadOnlyDictionary<Uri, IEdmModel> Build()
        {
            var result = new Dictionary<Uri, IEdmModel>();

            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<QueryingMetadataIdentity>(out metadataSet))
            {
                return result;
            }

            var contexts = metadataSet.Metadata.Values.OfType<BoundedContextElement>();
            foreach (var context in contexts)
            {
                var model = Build(context);
                _edmModelAnnotator.Annotate(context, model, model);

                result.Add(context.Identity.Id, model);
            }

            return result;
        }

        private IEdmModel Build(BoundedContextElement context)
        {
            if (context.ConceptualModel == null)
            {
                throw new InvalidOperationException("The conceptual model is not specified.");
            }

            return BuildModel(context.ResolveFullName(), context.ConceptualModel);
        }

        private IEdmModel BuildModel(string namespaceName, StructuralModelElement conceptualModelElement)
        {
            var model = new EdmModel();

            var container = new EdmEntityContainer(namespaceName, DefaultContainerName);
            model.AddElement(container);

            var typeBuilder = new TypeBuilder(namespaceName);
            BuildEntitySets(container, conceptualModelElement.RootEntities, typeBuilder);

            BuildTypes(model, conceptualModelElement.Entities, typeBuilder);

            return model;
        }

        private void BuildEntitySets(EdmEntityContainer container, IEnumerable<EntityElement> rootElements, TypeBuilder typeBuilder)
        {
            foreach (var rootEntity in rootElements)
            {
                var entitySetName = rootEntity.EntitySetName ?? rootEntity.ResolveName();
                var schemaType = typeBuilder.BuildSchemaType(rootEntity);

                container.AddEntitySet(entitySetName, (IEdmEntityType)schemaType);
            }
        }

        private void BuildTypes(EdmModel model, IEnumerable<EntityElement> entityElements, TypeBuilder typeBuilder)
        {
            foreach (var entityElement in entityElements)
            {
                var entityType = typeBuilder.BuildSchemaType(entityElement);
                model.AddElement(entityType);
                _edmModelAnnotator.Annotate(entityElement, entityType, model);

                var enumTypeElements = entityElement.Properties.Select(x => x.PropertyType).OfType<EnumTypeElement>();
                foreach (var enumTypeElement in enumTypeElements)
                {
                    var enumType = typeBuilder.BuildSchemaType(enumTypeElement);
                    model.AddElement(enumType);
                    _edmModelAnnotator.Annotate(enumTypeElement, enumType, model);
                }
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
    }
}
