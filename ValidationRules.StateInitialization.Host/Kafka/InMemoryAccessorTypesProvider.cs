using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Factories;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    internal sealed class InMemoryAccessorTypesProvider : IAccessorTypesProvider
    {
        private static readonly Lazy<IReadOnlyDictionary<Type, Type[]>> AccessorTypes = new Lazy<IReadOnlyDictionary<Type, Type[]>>(LoadAccessorTypes);

        private static IReadOnlyDictionary<Type, Type[]> LoadAccessorTypes()
            => AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Where(x => !x.IsDynamic)
                        .SelectMany(SafeGetAssemblyExportedTypes)
                        .SelectMany(type => type.GetInterfaces(), (type, @interface) => new { type, @interface })
                        .Where(x => !x.type.IsAbstract && x.@interface.IsGenericType && x.@interface.GetGenericTypeDefinition() == typeof(IMemoryBasedDataObjectAccessor<>))
                        .Select(x => new { GenericArgument = x.@interface.GetGenericArguments()[0], Type = x.type })
                        .GroupBy(x => x.GenericArgument, x => x.Type)
                        .ToDictionary(x => x.Key, x => x.ToArray());

        private static IEnumerable<Type> SafeGetAssemblyExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.ExportedTypes;
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        public IReadOnlyCollection<Type> GetAccessorsFor(Type dataObjectType) => AccessorTypes.Value
                                                                                              .TryGetValue(dataObjectType, out Type[] result)
                                                                                     ? result
                                                                                     : Array.Empty<Type>();
    }
}