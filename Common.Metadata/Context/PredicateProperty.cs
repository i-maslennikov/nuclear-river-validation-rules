using System;
using System.Collections.Generic;
using System.Globalization;

namespace NuClear.River.Common.Metadata.Context
{
    public static class PredicateProperty
    {
        public static readonly IPredicateProperty<string> Type = new PredicateProperty<string>("type");
        public static readonly IPredicateProperty<int> EntityType = new PredicateProperty<int>("entityType");
        public static readonly IPredicateProperty<long> EntityId = new PredicateProperty<long>("entityId");
        public static readonly IPredicateProperty<long> ProjectId = new PredicateProperty<long>("projectId");
        public static readonly IPredicateProperty<long> CategoryId = new PredicateProperty<long>("categoryId");
    }

    /// <summary>
    /// Поддерживает только примитивные типы, 
    /// для поддержки нестандартного типа требуется реализация IPredicateProperty
    /// </summary>
    public sealed class PredicateProperty<T> : IPredicateProperty<T>
        // where T: IConvertible - interface is missing in portable class library Profile151
    {
        public PredicateProperty(string name)
        {
            Name = name;
            Type = typeof(T);
        }

        public string Name { get; }
        public Type Type { get; }

        public T GetValue(Predicate p)
        {
            string value;
            if (!p.Properties.TryGetValue(Name, out value))
            {
                throw new ArgumentException($"Property {Name} is missing in predicate");
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        public void SetValue(IDictionary<string, string> properties, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            properties[Name] = Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}