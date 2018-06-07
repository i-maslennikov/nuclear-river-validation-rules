using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Mapping;

using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.SingleCheck.Store;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.SingleCheck
{
    public class PipelineFactory
    {
        private static readonly MappingSchema Facts = Schema.Facts;
        private static readonly MappingSchema Aggregates = Schema.Aggregates;
        private static readonly MappingSchema Messages = Schema.Messages;
        private static readonly MappingSchema WebApp = Schema.WebApp;

        private static readonly Lazy<IReadOnlyDictionary<MappingSchema, List<Type>>> AccessorTypes =
            new Lazy<IReadOnlyDictionary<MappingSchema, List<Type>>>(() => ScanForAccessors(new[] { Facts, Aggregates, Messages }));

        private static readonly Lazy<IReadOnlyCollection<Type>> DataObjectypes =
            new Lazy<IReadOnlyCollection<Type>>(() => AccessorTypes.Value
                                                                   .SelectMany(x => x.Value)
                                                                   .Select(x => x.GetInterfaces().Single(IsAccessorInterface))
                                                                   .Select(GetAccessorDataObject)
                                                                   .Distinct()
                                                                   .ToArray());

        public Pipeline Create()
        {
            var explicitlyAppenedTypes = new[]
                {
                    typeof(Ruleset),
                    typeof(Ruleset.AssociatedRule),
                    typeof(Ruleset.DeniedRule),
                    typeof(Ruleset.QuantitativeRule),
                    typeof(Ruleset.RulesetProject)
                };

            var dataObjectTypes = DataObjectypes.Value
                                                .Concat(explicitlyAppenedTypes)
                                                .ToList();

            var pool = new SchemaManager(new MappingSchema(Facts, Aggregates, Messages, WebApp), dataObjectTypes);
            return new Pipeline(AccessorTypes.Value[Facts], AccessorTypes.Value[Aggregates], AccessorTypes.Value[Messages], pool);
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
                    if (schema.GetAttribute<TableAttribute>(dataObjectType) != null)
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