using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace ValidationRules.Replication.DatabaseComparison
{
    internal static class TypeProvider
    {
        public static readonly MappingSchema Erm = Schema.Erm;
        public static readonly MappingSchema Facts = Schema.Facts;
        public static readonly MappingSchema Aggregates = Schema.Aggregates;
        public static readonly MappingSchema Messages = Schema.Messages;

        private static readonly Lazy<IReadOnlyDictionary<MappingSchema, Dictionary<Type, List<Type>>>> AccessorTypes =
            new Lazy<IReadOnlyDictionary<MappingSchema, Dictionary<Type, List<Type>>>>(() => ScanForAccessors(new[] { Facts, Aggregates, Messages }));

        public static IEnumerable<Type> GetDataObjectTypes(MappingSchema schema)
            => AccessorTypes.Value[schema].Keys;

        public static IEnumerable<Type> GetAccessors(MappingSchema schema, Type dataObjectType)
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
                        List<Type> accessors;
                        if (!result[schema].TryGetValue(dataObjectType, out accessors))
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