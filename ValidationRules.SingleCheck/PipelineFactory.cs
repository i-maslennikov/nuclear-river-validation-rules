using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.SingleCheck
{
    public class PipelineFactory
    {
        public static readonly MappingSchema Erm = Schema.Erm;
        public static readonly MappingSchema Facts = Schema.Facts;
        public static readonly MappingSchema Aggregates = Schema.Aggregates;
        public static readonly MappingSchema Messages = Schema.Messages;

        public static readonly IReadOnlyDictionary<MappingSchema, List<Type>> AccessorTypes = ScanForAccessors(new[] { Facts, Aggregates, Messages });

        public Pipeline CreatePipeline()
        {
            return new Pipeline(AccessorTypes[Facts], AccessorTypes[Aggregates], AccessorTypes[Messages]);
        }

        private static IReadOnlyDictionary<MappingSchema, List<Type>> ScanForAccessors(IReadOnlyCollection<MappingSchema> schemata)
        {
            var accessorTypes = typeof(IValidationResultAccessor).Assembly.GetTypes().Where(IsAccessorImplementation);

            var result = schemata.ToDictionary(x => x, x => new List<Type>());

            foreach (var accessorType in accessorTypes)
            {
                var interfaceType = accessorType.GetInterfaces().Single(IsAccessorInterface);
                var dataObjectType = GetAccessorDataObject(interfaceType);

                foreach (var schema in schemata)
                {
                    if (!string.IsNullOrEmpty(schema.GetEntityDescriptor(dataObjectType).SchemaName))
                    {
                        result[schema].Add(accessorType);
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