using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Replication;

namespace ValidationRules.Replication.DatabaseComparison.Tests
{
    internal static class TypeProvider
    {
        private static readonly Lazy<IReadOnlyDictionary<MappingSchema, Dictionary<Type, List<Type>>>> AccessorTypes =
            new Lazy<IReadOnlyDictionary<MappingSchema, Dictionary<Type, List<Type>>>>(() => ScanForAccessors(new[]
                {
                    StorageDescriptor.Facts.MappingSchema,
                    StorageDescriptor.Aggregates.MappingSchema,
                    StorageDescriptor.Messages.MappingSchema,
                }));

        public static IEnumerable<Type> GetDataObjectTypes(MappingSchema schema)
            => AccessorTypes.Value[schema].Keys;

        public static IEnumerable<Type> GetAccessorTypes(MappingSchema schema, Type dataObjectType)
            => AccessorTypes.Value[schema][dataObjectType];

        private static IReadOnlyDictionary<MappingSchema, Dictionary<Type, List<Type>>> ScanForAccessors(IReadOnlyCollection<MappingSchema> schemata)
        {
            var accessorTypes = typeof(IValidationResultAccessor).Assembly.GetTypes().Where(IsAccessorImplementation);

            var result = schemata.ToDictionary(x => x, x => new Dictionary<Type, List<Type>>());

            foreach (var accessorType in accessorTypes)
            {
                var interfaceType = accessorType.GetInterfaces().Single(IsAccessorInterface);
                var dataObjectType = GetAccessorDataObject(interfaceType);

                foreach (var schema in schemata)
                {
                    if (schema.GetAttribute<TableAttribute>(dataObjectType) != null)
                    {
                        if (!result[schema].TryGetValue(dataObjectType, out var accessors))
                        {
                            accessors = new List<Type>();
                            result[schema].Add(dataObjectType, accessors);
                        }

                        accessors.Add(accessorType);
                        break;
                    }
                }
            }

            return result;
        }

        private static bool IsAccessorImplementation(Type type)
            => type.IsClass && !type.IsAbstract && type.GetInterfaces().Any(IsAccessorInterface);

        private static bool IsAccessorInterface(Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>);

        private static Type GetAccessorDataObject(Type type)
            => type.GetGenericArguments().Single();
    }
}