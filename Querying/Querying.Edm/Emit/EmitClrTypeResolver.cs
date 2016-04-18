using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Edm.EF;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.Querying.Edm.Emit
{
    public sealed class EmitClrTypeResolver : IClrTypeBuilder, IClrTypeProvider
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly Dictionary<IMetadataElementIdentity, Type> _typesById = new Dictionary<IMetadataElementIdentity, Type>();

        public EmitClrTypeResolver(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public void Build()
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<QueryingMetadataIdentity>(out metadataSet))
            {
                return;
            }

            var contexts = metadataSet.Metadata.Values.OfType<BoundedContextElement>();
            foreach (var context in contexts)
            {
                var assemblyName = $"{context.ResolveFullName()}.Domain";
                var moduleBuilder = EmitHelper.DefineAssembly(assemblyName).DefineModule(assemblyName);
                foreach (var entityElement in context.ConceptualModel.Entities)
                {
                    BuildType(moduleBuilder, entityElement);
                }
            }
        }

        public Type BuildType(ModuleBuilder moduleBuilder, EntityElement entityElement)
        {
            Type type;
            if (!_typesById.TryGetValue(entityElement.Identity, out type))
            {
                _typesById.Add(entityElement.Identity, type = CreateType(moduleBuilder, entityElement));
            }

            return type;
        }

        public Type Get(IMetadataElementIdentity elementIdentity)
        {
            Type type;
            return _typesById.TryGetValue(elementIdentity, out type) ? type : null;
        }

        private static Type ConvertType(ElementaryTypeKind propertyType)
        {
            switch (propertyType)
            {
                case ElementaryTypeKind.Byte:
                    return typeof(byte);
                case ElementaryTypeKind.Int16:
                case ElementaryTypeKind.Int32:
                    return typeof(int);
                case ElementaryTypeKind.Int64:
                    return typeof(long);
                case ElementaryTypeKind.Single:
                    return typeof(float);
                case ElementaryTypeKind.Double:
                    return typeof(double);
                case ElementaryTypeKind.Decimal:
                    return typeof(decimal);
                case ElementaryTypeKind.Boolean:
                    return typeof(bool);
                case ElementaryTypeKind.String:
                    return typeof(string);
                case ElementaryTypeKind.DateTimeOffset:
                    return typeof(DateTimeOffset);
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyType));
            }
        }

        private static Type CreateRelationType(Type entityType, EntityRelationCardinality cardinality)
        {
            switch (cardinality)
            {
                case EntityRelationCardinality.One:
                case EntityRelationCardinality.OptionalOne:
                    return entityType;
                case EntityRelationCardinality.Many:
                    return typeof(ICollection<>).MakeGenericType(entityType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardinality));
            }
        }

        private Type CreateType(ModuleBuilder moduleBuilder, EntityElement entityElement)
        {
            var typeName = entityElement.ResolveFullName();
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);

            foreach (var propertyElement in entityElement.Properties)
            {
                var propertyName = propertyElement.ResolveName();
                var propertyType = ResolveType(moduleBuilder, propertyElement);

                typeBuilder.DefineProperty(propertyName, propertyType);
            }

            foreach (var relationElement in entityElement.Relations)
            {
                var relationTarget = relationElement.Target;
                var relationCardinality = relationElement.Cardinality;

                var propertyName = relationElement.ResolveName();
                var propertyType = CreateRelationType(BuildType(moduleBuilder, relationTarget), relationCardinality);

                typeBuilder.DefineProperty(propertyName, propertyType);
            }

            return typeBuilder.CreateType();
        }

        private Type ResolveType(ModuleBuilder moduleBuilder, EntityPropertyElement propertyElement)
        {
            var propertyType = propertyElement.PropertyType;

            Type resolvedType;
            if (propertyType.TypeKind == StructuralModelTypeKind.Primitive)
            {
                resolvedType = ConvertType(((PrimitiveTypeElement)propertyType).PrimitiveType);
            }
            else if (propertyType.TypeKind == StructuralModelTypeKind.Enum)
            {
                if (!_typesById.TryGetValue(propertyType.Identity, out resolvedType))
                {
                    _typesById.Add(propertyType.Identity, resolvedType = CreateEnum(moduleBuilder, (EnumTypeElement)propertyType));
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            if (propertyElement.IsNullable && resolvedType.IsValueType)
            {
                return typeof(Nullable<>).MakeGenericType(resolvedType);
            }

            return resolvedType;
        }

        private static Type CreateEnum(ModuleBuilder moduleBuilder, EnumTypeElement element)
        {
            var typeName = element.ResolveName();
            var underlyingType = ConvertType(element.UnderlyingType);

            var typeBuilder = moduleBuilder.DefineEnum(typeName, underlyingType);

            foreach (var member in element.Members)
            {
                typeBuilder.DefineLiteral(member.Key, Convert.ChangeType(member.Value, underlyingType));
            }

            return typeBuilder.CreateType();
        }
    }
}